using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionState
{
    Idle, Attack, Aim
}

public class AgentAction : MonoBehaviour
{
    public event Action<ActionState> OnStateChange;

    AgentEquipment equipment;
    AgentController controller;

    State currentState;
    Dictionary<Type, State> availableStates;
    Dictionary<Type, ActionState> stateTranslator = new Dictionary<Type, ActionState>()
    {
        { typeof(IdleState), ActionState.Idle },
        { typeof(AttackState), ActionState.Attack },
        { typeof(AimState), ActionState.Aim },
    };

    private void Awake()
    {
        equipment = GetComponent<AgentEquipment>();
        controller = GetComponent<AgentController>();
        availableStates = new Dictionary<Type, State>()
        {
            { typeof(IdleState), new IdleState(this) },
            { typeof(AttackState), new AttackState(this) },
            { typeof(AimState), new AimState(this) },
        };
        currentState = availableStates[typeof(IdleState)];
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
    }

    private void FixedUpdate()
    {
        currentState.DuringPhysics();
    }

    abstract class State
    {
        protected AgentAction action;
        protected GameObject gameObject;
        protected Transform transform;

        public State(AgentAction action)
        {
            this.action = action;
            gameObject = action.gameObject;
            transform = action.transform;
        }

        public virtual void Before() { }
        public virtual void During() { }
        public virtual void DuringPhysics() { }
        public virtual void After() { }
        public virtual Type CheckTransitions() { return null; }
    }

    class IdleState : State
    {
        public IdleState(AgentAction action) : base(action) { }

        public override Type CheckTransitions()
        {
            if (action.controller.Attack)
            {
                return typeof(AttackState);
            }
            if (action.controller.Aim)
            {
                return typeof(AimState);
            }
            return null;
        }
    }

    class AttackState : State
    {
        public AttackState(AgentAction action) : base(action) { }

        public override void Before()
        {
            action.equipment.CurrentWeaponAttack.BeginAttack();
        }

        public override void During()
        {
            action.equipment.CurrentWeaponAttack.DuringAttack();
        }

        public override void After()
        {
            action.equipment.CurrentWeaponAttack.EndAttack();
        }

        public override Type CheckTransitions()
        {
            if (!action.controller.Attack)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

    class AimState : State
    {
        public AimState(AgentAction action) : base(action) { }

        public override Type CheckTransitions()
        {
            if (!action.controller.Aim)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

}
