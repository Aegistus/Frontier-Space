using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float crouchSpeed = 1f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float jumpVelocity = 5f;
    [SerializeField] float collisionDistance = .5f;

    AgentController controller;
    HumanoidAnimator humanoidAnimator;

    State currentState;
    Dictionary<Type, State> availableStates;

    float verticalVelocity = 0f;
    Vector3 velocity;
    float groundCheckRadius = .5f;
    Vector3 groundCheckHeight = new Vector3(0, .4f, 0);
    float obstacleCheckHeight = .2f;

    private void Awake()
    {
        controller = GetComponent<AgentController>();
        humanoidAnimator = GetComponentInChildren<HumanoidAnimator>();
        availableStates = new Dictionary<Type, State>()
        {
            {typeof(StandState), new StandState(this) },
            {typeof(WalkState), new WalkState(this) },
            {typeof(JumpState), new JumpState(this) },
            {typeof(FallState), new FallState(this) },
            {typeof(RunState), new RunState(this) },
            {typeof(CrouchState), new CrouchState(this) },

        };
        currentState = availableStates[typeof(StandState)];
    }

    private void Start()
    {
        currentState.Before();
    }

    private void Update()
    {
        currentState.During();
        Type nextState = currentState.CheckTransitions();
        if (nextState != null)
        {
            currentState.After();
            currentState = availableStates[nextState];
            currentState.Before();
        }
        transform.LookAt(controller.Target);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position + groundCheckHeight, groundCheckRadius, groundLayer))
        {
            return true;
        }
        return false;
    }

    public void Move(Vector3 direction, float speed)
    {
        direction.Normalize();
        velocity = speed * direction;
        Vector3 obstacleCheckPoint = transform.position;
        obstacleCheckPoint.y += obstacleCheckHeight;
        if (!Physics.Raycast(obstacleCheckPoint, transform.TransformDirection(direction), collisionDistance))
        {
            transform.Translate(velocity * Time.deltaTime, Space.Self);
        }
    }


    private void FixedUpdate()
    {
        currentState.DuringPhysics();
        transform.Translate(verticalVelocity * Time.deltaTime * Vector3.up);
        if (!IsGrounded())
        {
            verticalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }
    }

    abstract class State
    {
        protected AgentMovement movement;
        protected GameObject gameObject;
        protected Transform transform;

        public State(AgentMovement movement)
        {
            this.movement = movement;
            gameObject = movement.gameObject;
            transform = movement.transform;
        }

        public virtual void Before() { }
        public virtual void During() { }
        public virtual void DuringPhysics() { }
        public virtual void After() { }
        public virtual Type CheckTransitions() { return null; }
    }

    class StandState : State
    {
        public StandState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            print("Standing");
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Idle, false);
        }

        public override Type CheckTransitions()
        {
            if (movement.controller.MovementInput)
            {
                return typeof(WalkState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            if (movement.controller.Crouch)
            {
                return typeof(CrouchState);
            }
            return null;
        }
    }

    class WalkState : State
    {
        public WalkState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            print("Walking");
        }

        public override void DuringPhysics()
        {
            movement.Move(movement.controller.GetMovementInput(), movement.walkSpeed);
        }

        public override void During()
        {
            if (movement.controller.Forwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkForwards, false);
            }
            else if (movement.controller.Backwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkBackwards, false);
            }
            else if (movement.controller.Left)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkLeft, false);
            }
            else if (movement.controller.Right)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkRight, false);
            }
        }

        public override Type CheckTransitions()
        {
            if (movement.controller.NoMovementInput)
            {
                return typeof(StandState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            if (movement.controller.Run)
            {
                return typeof(RunState);
            }
            if (movement.controller.Crouch)
            {
                return typeof(CrouchState);
            }
            return null;
        }
    }

    class JumpState : State
    {
        float timer = 0f;
        float fallDelay = .5f;
        Vector3 initialDirection;
        float initialSpeed;

        public JumpState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            timer = 0f;
            movement.verticalVelocity += movement.jumpVelocity;
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Jump, false);
            print(movement.velocity);
            initialDirection = movement.velocity;
            initialSpeed = initialDirection.magnitude;
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override void DuringPhysics()
        {
            movement.Move(initialDirection, initialSpeed);
        }

        public override Type CheckTransitions()
        {
            if (timer >= fallDelay)
            {
                return typeof(FallState);
            }
            return null;
        }
    }

    class FallState : State
    {
        Vector3 initialDirection;
        float initialSpeed;

        public FallState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Fall, false);
            print(movement.velocity);
            initialDirection = movement.velocity;
            initialSpeed = initialDirection.magnitude;
        }

        public override void DuringPhysics()
        {
            movement.Move(initialDirection, initialSpeed);
        }

        public override Type CheckTransitions()
        {
            if (movement.IsGrounded())
            {
                return typeof(StandState);
            }
            return null;
        }
    }

    class RunState : State
    {
        public RunState(AgentMovement movement) : base(movement) { }

        public override void DuringPhysics()
        {
            movement.Move(movement.controller.GetMovementInput(), movement.runSpeed);
        }

        public override void During()
        {
            if (movement.controller.Left)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.RunLeft, false);
            }
            else if (movement.controller.Right)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.RunRight, false);
            }
            else
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.RunForwards, false);
            }
        }

        public override Type CheckTransitions()
        {
            if (movement.controller.NoMovementInput)
            {
                return typeof(StandState);
            }
            if (!movement.controller.Run)
            {
                return typeof(WalkState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            return null;
        }
    }

    class CrouchState : State
    {
        public CrouchState(AgentMovement movement) : base(movement) { }

        public override void DuringPhysics()
        {
            movement.Move(movement.controller.GetMovementInput(), movement.crouchSpeed);
        }

        public override void During()
        {
            if (movement.controller.Forwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchForwards, false);
            }
            else if (movement.controller.Backwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchBackwards, false);
            }
            else if (movement.controller.Left)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchLeft, false);
            }
            else if (movement.controller.Right)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchRight, false);
            }
            else
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchIdle, false);
            }
        }

        public override Type CheckTransitions()
        {
            if (!movement.controller.Crouch)
            {
                return typeof(StandState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            return null;
        }
    }
}