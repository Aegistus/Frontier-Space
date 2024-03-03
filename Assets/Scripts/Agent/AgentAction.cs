using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction : MonoBehaviour
{
    [SerializeField] Transform eyeTransform;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] float switchWeaponTime = .5f;
    [SerializeField] float grenadeThrowForce = 5f;

    public event Action<Type> OnStateChange;
    public static float InteractDistance { get; private set; }
    public Type CurrentState => currentState.GetType();

    readonly int numOfFlinchAnimations = 5;
    AgentEquipment equipment;
    AgentController controller;
    AgentMovement movement;
    HumanoidAnimator agentAnimator;
    HumanoidIK agentIK;

    Grenade currentGrenade;

    State currentState;
    Dictionary<Type, State> availableStates;

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
            { typeof(HoldGrenadeState), new HoldGrenadeState(this) },
            { typeof(ThrowGrenadeState), new ThrowGrenadeState(this) },
            { typeof(MeleeAttackState), new MeleeAttackState(this) },
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
            ChangeState(nextState);
        }
    }

    private void FixedUpdate()
    {
        currentState.DuringPhysics();
    }

    void ChangeState(Type nextState)
    {
        currentState.After();
        currentState = availableStates[nextState];
        OnStateChange?.Invoke(nextState);
        currentState.Before();
    }

    public abstract class State
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
    public partial class IdleState : State { }
    public partial class AttackState : State { }
    public partial class AimState : State { }
    public partial class AimAttackState : State { }
    public partial class InteractState : State { }
    public partial class ReloadState : State { }
    public partial class SwitchWeaponState : State { }
    public partial class HoldGrenadeState : State { }
    public partial class ThrowGrenadeState : State { }
    public partial class MeleeAttackState : State { }

}
