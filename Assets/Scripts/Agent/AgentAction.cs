using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionState
{
    Idle, Attack, Aim, Interact
}

public class AgentAction : MonoBehaviour
{
    [SerializeField] Transform lookTransform;
    [SerializeField] float interactDistance = 2f;

    public event Action<ActionState> OnStateChange;

    AgentEquipment equipment;
    AgentController controller;
    AgentMovement movement;

    State currentState;
    Dictionary<Type, State> availableStates;
    Dictionary<Type, ActionState> stateTranslator = new Dictionary<Type, ActionState>()
    {
        { typeof(IdleState), ActionState.Idle },
        { typeof(AttackState), ActionState.Attack },
        { typeof(AimState), ActionState.Aim },
        { typeof(InteractState), ActionState.Interact },
    };

    private void Awake()
    {
        equipment = GetComponent<AgentEquipment>();
        controller = GetComponent<AgentController>();
        movement = GetComponent<AgentMovement>();
        availableStates = new Dictionary<Type, State>()
        {
            { typeof(IdleState), new IdleState(this) },
            { typeof(AttackState), new AttackState(this) },
            { typeof(AimState), new AimState(this) },
            { typeof(InteractState), new InteractState(this) },
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
        protected AgentMovement movement;
        protected GameObject gameObject;
        protected Transform transform;

        public State(AgentAction action)
        {
            this.action = action;
            movement = action.movement;
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
            if (movement.CurrentState != MovementState.Run)
            {
                if (action.controller.Attack)
                {
                    return typeof(AttackState);
                }
                if (action.controller.Aim)
                {
                    return typeof(AimState);
                }
            }
            if (action.controller.Interact)
            {
                return typeof(InteractState);
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
            if (movement.CurrentState == MovementState.Run)
            {
                return typeof(IdleState);
            }
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

        public override void Before()
        {
            action.equipment.SetWeaponOffset(WeaponOffset.Aiming);
        }

        public override void After()
        {
            if (movement.CurrentState != MovementState.Run)
            {
                action.equipment.SetWeaponOffset(WeaponOffset.Idle);
            }
        }

        public override Type CheckTransitions()
        {
            if (movement.CurrentState == MovementState.Run)
            {
                return typeof(IdleState);
            }
            if (!action.controller.Aim)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

    class InteractState : State
    {
        public InteractState(AgentAction action) : base(action) { }

        public override void Before()
        {
            RaycastHit rayHit;
            Physics.Raycast(action.lookTransform.position, action.lookTransform.forward, out rayHit, action.interactDistance);
            if (rayHit.collider != null)
            {
                IInteractable interactable = rayHit.collider.GetComponentInChildren<IInteractable>();
                interactable?.Interact();
            }
        }

        public override Type CheckTransitions()
        {
            return typeof(IdleState);
        }
    }
}
