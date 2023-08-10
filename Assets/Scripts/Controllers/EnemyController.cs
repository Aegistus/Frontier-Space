using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : AgentController
{
    State currentState;
    Dictionary<Type, State> availableStates;

    private void Awake()
    {
        availableStates = new Dictionary<Type, State>()
        {

        };

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

    public override void FindNewTarget()
    {
        
    }

    abstract class State
    {
        protected EnemyController controller;
        protected GameObject gameObject;
        protected Transform transform;

        public State(EnemyController controller)
        {
            this.controller = controller;
            gameObject = controller.gameObject;
            transform = controller.transform;
        }

        public virtual void Before() { }
        public virtual void During() { }
        public virtual void DuringPhysics() { }
        public virtual void After() { }
        public virtual Type CheckTransitions() { return null; }
    }
}
