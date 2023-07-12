using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float jumpVelocity = 5f;

    AgentController controller;
    HumanoidAnimator humanoidAnimator;

    State currentState;
    Dictionary<Type, State> availableStates;

    float horizontalVelocity = 0f;
    float groundCheckRadius = .5f;
    Vector3 groundCheckHeight = new Vector3(0, .4f, 0);

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
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position + groundCheckHeight, groundCheckRadius, groundLayer))
        {
            return true;
        }
        return false;
    }


    private void FixedUpdate()
    {
        currentState.DuringPhysics();
        transform.Translate(horizontalVelocity * Time.deltaTime * Vector3.up);
        if (!IsGrounded())
        {
            horizontalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
        }
        else if (horizontalVelocity < 0)
        {
            horizontalVelocity = 0;
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
            return null;
        }
    }

    class WalkState : State
    {
        Vector3 input;

        public WalkState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            print("Walking");
        }

        public override void DuringPhysics()
        {
            input = movement.controller.GetMovementInput();
            transform.Translate(movement.walkSpeed * Time.deltaTime * input, Space.Self);
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
            return null;
        }
    }

    class JumpState : State
    {
        float timer = 0f;
        float fallDelay = .5f;

        public JumpState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            timer = 0f;
            movement.horizontalVelocity += movement.jumpVelocity;
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Jump, false);
        }

        public override void During()
        {
            timer += Time.deltaTime;
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
        public FallState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Fall, false);
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
}