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
    CharacterController charController;

    State currentState;
    Dictionary<Type, State> availableStates;

    float horizontalVelocity = 0f;
    float groundCheckRadius = .5f;
    Vector3 groundCheckHeight = new Vector3(0, .4f, 0);

    private void Awake()
    {
        controller = GetComponent<AgentController>();
        humanoidAnimator = GetComponentInChildren<HumanoidAnimator>();
        charController = GetComponent<CharacterController>();
        availableStates = new Dictionary<Type, State>()
        {
            {typeof(StandState), new StandState(this) },
            {typeof(WalkState), new WalkState(this) },
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
        if (!IsGrounded())
        {
            transform.Translate(horizontalVelocity * Time.deltaTime * Vector3.up);
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
            return null;
        }
    }

    class JumpState : State
    {
        public JumpState(AgentMovement movement) : base(movement) { }


    }
}