using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionState
{
    Idle, Attack, Aim, Interact, AimAttack, Reload, SwitchWeapon
}

public class AgentAction : MonoBehaviour
{
    [SerializeField] Transform lookTransform;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] float switchWeaponTime = .5f;

    public event Action<ActionState> OnStateChange;
    public static float InteractDistance { get; private set; }

    AgentEquipment equipment;
    AgentController controller;
    AgentMovement movement;
    HumanoidAnimator agentAnimator;
    HumanoidIK agentIK;

    State currentState;
    Dictionary<Type, State> availableStates;
    Dictionary<Type, ActionState> stateTranslator = new Dictionary<Type, ActionState>()
    {
        { typeof(IdleState), ActionState.Idle },
        { typeof(AttackState), ActionState.Attack },
        { typeof(AimState), ActionState.Aim },
        { typeof(AimAttackState), ActionState.AimAttack },
        { typeof(ReloadState), ActionState.Reload },
        { typeof(InteractState), ActionState.Interact },
        { typeof(SwitchWeaponState), ActionState.SwitchWeapon },
    };

    private void Awake()
    {
        equipment = GetComponent<AgentEquipment>();
        controller = GetComponent<AgentController>();
        movement = GetComponent<AgentMovement>();
        agentAnimator = GetComponentInChildren<HumanoidAnimator>();
        agentIK = GetComponentInChildren<HumanoidIK>();
        availableStates = new Dictionary<Type, State>()
        {
            { typeof(IdleState), new IdleState(this) },
            { typeof(AttackState), new AttackState(this) },
            { typeof(AimState), new AimState(this) },
            { typeof(AimAttackState), new AimAttackState(this) },
            { typeof(InteractState), new InteractState(this) },
            { typeof(ReloadState), new ReloadState(this) },
            { typeof(SwitchWeaponState), new SwitchWeaponState(this) },
        };
        currentState = availableStates[typeof(IdleState)];
        InteractDistance = interactDistance;
        GetComponent<AgentHealth>().OnAgentDeath += () => enabled = false;
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
            if (action.equipment.HasWeaponEquipped)
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
                if (action.controller.Reload && action.equipment.CurrentWeaponAmmunition.CurrentCarriedAmmo > 0)
                {
                    return typeof(ReloadState);
                }
            }
            if (action.controller.Interact)
            {
                return typeof(InteractState);
            }
            if (action.controller.SwitchWeapon)
            {
                return typeof(SwitchWeaponState);
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
            if (action.controller.Attack)
            {
                return typeof(AimAttackState);
            }
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

    class AimAttackState : State
    {
        public AimAttackState(AgentAction action) : base(action) { }

        public override void Before()
        {
            action.equipment.CurrentWeaponAttack.BeginAttack();
            action.equipment.SetWeaponOffset(WeaponOffset.Aiming);
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
                return typeof(AimState);
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
                interactable?.Interact(gameObject);
            }
        }

        public override Type CheckTransitions()
        {
            return typeof(IdleState);
        }
    }

    class ReloadState : State
    {
        bool successful;

        public ReloadState(AgentAction action) : base(action) { }

        public override void Before()
        {
            successful = action.equipment.CurrentWeaponAmmunition.TryReload();
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.UpperReload);
            action.agentIK.SetHandWeight(Hand.Left, 0);
            action.equipment.SetWeaponOffset(WeaponOffset.Reloading);
        }

        public override void After()
        {
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.None);
            action.agentIK.SetHandWeight(Hand.Left, 1);
            action.equipment.SetWeaponOffset(WeaponOffset.Idle);
        }

        public override Type CheckTransitions()
        {
            if (!successful || !action.equipment.CurrentWeaponAmmunition.Reloading)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

    class SwitchWeaponState : State
    {
        bool failure;
        float timer = 0f;

        public SwitchWeaponState(AgentAction action) : base(action) { }

        public override void Before()
        {
            timer = 0f;
            failure = action.equipment.TrySwitchWeapon();
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override Type CheckTransitions()
        {
            if (failure)
            {
                return typeof(IdleState);
            }
            if (timer >= action.switchWeaponTime)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }
}
