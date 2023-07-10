using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;

    AgentController controller;
    HumanoidAnimator humanoidAnimator;
    State currentState;
    Dictionary<Type, State> availableStates;

    private void Awake()
    {
        controller = GetComponent<AgentController>();
        humanoidAnimator = GetComponentInChildren<HumanoidAnimator>();
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


    abstract class State
    {
        protected AgentMovement movement;

        public State(AgentMovement movement)
        {
            this.movement = movement;
        }

        public virtual void Before() { }
        public virtual void During() { }
        public virtual void DuringPhysics() { }
        public virtual void After() { }
        public virtual Type CheckTransitions() { return null; }
    }

    class StandState : State
    {
        public StandState(AgentMovement movement) : base(movement)
        {

        }

        public override void Before()
        {
            print("Standing");
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
        public WalkState(AgentMovement movement) : base(movement)
        {

        }

        public override void Before()
        {
            print("Walking");
        }
    }
}