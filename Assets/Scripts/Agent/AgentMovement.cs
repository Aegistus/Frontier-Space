using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MovementState
{
    Stand, Walk, Jump, Fall, Run, Crouch
}

public class AgentMovement : MonoBehaviour
{
    public event Action<MovementState> OnStateChange;
    public MovementState CurrentState => stateTranslator[currentState.GetType()];

    [SerializeField] LayerMask groundLayer;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float crouchSpeed = 1f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float jumpVelocity = 5f;
    [SerializeField] float collisionCheckDistance = .5f;

    AgentController controller;
    AgentEquipment equipment;
    HumanoidAnimator humanoidAnimator;

    State currentState;
    Dictionary<Type, State> availableStates;
    Dictionary<Type, MovementState> stateTranslator = new Dictionary<Type, MovementState>()
    {
        { typeof(StandState), MovementState.Stand},
        { typeof(WalkState), MovementState.Walk },
        { typeof(JumpState), MovementState.Jump },
        { typeof(FallState), MovementState.Fall },
        { typeof(RunState), MovementState.Run },
        { typeof(CrouchState), MovementState.Crouch },
    };

    float verticalVelocity = 0f;
    Vector3 velocity;
    float groundCheckRadius = .5f;
    Vector3 groundCheckHeight = new Vector3(0, .45f, 0);
    Vector3 ceilingCheckHeight = new Vector3(0, 2, 0);
    float obstacleCheckHeight = .5f;
    float obstacleCheckWidth = .25f;

    private void Awake()
    {
        controller = GetComponent<AgentController>();
        equipment = GetComponent<AgentEquipment>();
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
            OnStateChange?.Invoke(stateTranslator[nextState]);
            currentState.Before();
        }
        GroundPlayer();
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position + groundCheckHeight, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public bool HittingCeiling()
    {
        if (Physics.CheckSphere(transform.position + ceilingCheckHeight, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore))
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
        int hits = 0;
        for (float offset = -obstacleCheckWidth; offset <= obstacleCheckWidth; offset += obstacleCheckWidth / 2)
        {
            if (Physics.Raycast(obstacleCheckPoint + transform.right * offset, transform.TransformDirection(direction), collisionCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                hits++;
            }
        }
        if (hits < 2)
        {
            transform.Translate(velocity * Time.deltaTime, Space.Self);
        }
    }

    void GroundPlayer()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position + groundCheckHeight, -transform.up, out rayHit, groundCheckHeight.y - .05f, groundLayer))
        {
            transform.position = rayHit.point;
        }
    }


    private void FixedUpdate()
    {
        currentState.DuringPhysics();
        if (HittingCeiling() && verticalVelocity > 0)
        {
            verticalVelocity = 0;
        }
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
            movement.velocity = Vector3.zero;
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

        public override void Before()
        {
            movement.humanoidAnimator.SetRigWeight(.5f);
            movement.equipment.SetWeaponOffset(WeaponOffset.Running);
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

        public override void After()
        {
            movement.humanoidAnimator.SetRigWeight(1f);
            movement.equipment.SetWeaponOffset(WeaponOffset.Idle);
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

        public override void Before()
        {
            movement.equipment.SetWeaponOffset(WeaponOffset.Crouching);
        }

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

        public override void After()
        {
            movement.equipment.SetWeaponOffset(WeaponOffset.Idle);
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