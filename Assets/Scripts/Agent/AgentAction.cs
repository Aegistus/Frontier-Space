using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionState
{
    Idle, Attack, Aim, Interact, AimAttack, Reload, SwitchWeapon, HoldGrenade, ThrowGrenade, Melee
}

public class AgentAction : MonoBehaviour
{
    [SerializeField] Transform eyeTransform;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] float switchWeaponTime = .5f;
    [SerializeField] float grenadeThrowForce = 5f;

    public event Action<ActionState> OnStateChange;
    public static float InteractDistance { get; private set; }

    readonly int numOfFlinchAnimations = 5;
    AgentEquipment equipment;
    AgentController controller;
    AgentMovement movement;
    HumanoidAnimator agentAnimator;
    HumanoidIK agentIK;

    Grenade currentGrenade;

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
        { typeof(HoldGrenadeState), ActionState.HoldGrenade },
        { typeof(ThrowGrenadeState), ActionState.ThrowGrenade },
        { typeof(MeleeState), ActionState.Melee },
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
            { typeof(HoldGrenadeState), new HoldGrenadeState(this) },
            { typeof(ThrowGrenadeState), new ThrowGrenadeState(this) },
            { typeof(MeleeState), new MeleeState(this) },
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

    private void Flinch()
    {
        // pick a random flinch animation
        int randIndex = UnityEngine.Random.Range(0, numOfFlinchAnimations);
        agentAnimator.SetInteger("FlinchIndex", randIndex);
        agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.Flinch);
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

        public override void Before()
        {
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.None);
        }

        public override Type CheckTransitions()
        {
            if (action.equipment.HasWeaponEquipped)
            {
                if (movement.CurrentState != typeof(AgentMovement.RunState))
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
                if (action.controller.Reload)
                {
                    if (action.equipment.CurrentWeaponAmmunition.CurrentCarriedAmmo > 0 || action.equipment.CurrentWeaponAmmunition.InfiniteCarriedAmmo)
                    {
                        return typeof(ReloadState);
                    }
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
            if (action.controller.ArmGrenade)
            {
                return typeof(HoldGrenadeState);
            }
            if (action.controller.Melee)
            {
                return typeof(MeleeState);
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
            if (movement.CurrentState == typeof(AgentMovement.RunState))
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
            if (movement.CurrentState != typeof(AgentMovement.RunState))
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
            if (movement.CurrentState == typeof(AgentMovement.RunState))
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
            if (action.equipment.CurrentWeaponAttack is RangedWeaponAttack attack)
            {
                attack.Aimed = true;
            }
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
            if (action.equipment.CurrentWeaponAttack is RangedWeaponAttack attack)
            {
                attack.Aimed = false;
            }
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
            Physics.Raycast(action.eyeTransform.position, action.eyeTransform.forward, out rayHit, action.interactDistance);
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
            //action.agentIK.SetHandWeight(Hand.Left, 0);
            action.equipment.SetWeaponOffset(WeaponOffset.Reloading);
        }

        public override void After()
        {
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.None);
            //action.agentIK.SetHandWeight(Hand.Left, 1);
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
        bool success;
        float timer = 0f;

        public SwitchWeaponState(AgentAction action) : base(action) { }

        public override void Before()
        {
            timer = 0f;
            success = action.equipment.TrySwitchWeapon();
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override Type CheckTransitions()
        {
            if (!success)
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

    class HoldGrenadeState : State
    {
        public HoldGrenadeState(AgentAction action) : base(action) { }

        public override void Before()
        {
            action.currentGrenade = action.equipment.GetGrenade();
            if (action.currentGrenade != null)
            {
                action.equipment.UnEquip(action.equipment.CurrentWeapon);
                action.currentGrenade.gameObject.SetActive(true);
                action.currentGrenade.Arm();
                action.currentGrenade.transform.position = action.eyeTransform.position + .5f * action.eyeTransform.right;
                action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.UpperHoldGrenade);
                action.agentIK.SetHandWeight(Hand.Right, 0);
                action.agentIK.SetHandWeight(Hand.Left, 0);
            }
        }

        public override Type CheckTransitions()
        {
            if (action.currentGrenade == null)
            {
                return typeof(IdleState);
            }
            if (action.controller.ThrowGrenade)
            {
                return typeof(ThrowGrenadeState);
            }
            return null;
        }
    }

    class ThrowGrenadeState : State
    {
        float maxTimer = 1f;
        float timer = 0f;

        public ThrowGrenadeState(AgentAction action) : base(action) { }

        public override void Before()
        {
            timer = 0f;
            action.currentGrenade.transform.SetParent(null);
            Rigidbody grenRB = action.currentGrenade.GetComponent<Rigidbody>();
            grenRB.useGravity = true;
            grenRB.AddForce(action.eyeTransform.forward * action.grenadeThrowForce);
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.UpperThrowGrenade);
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override void After()
        {
            action.agentIK.SetHandWeight(Hand.Right, 1);
            action.agentIK.SetHandWeight(Hand.Left, 1);
        }

        public override Type CheckTransitions()
        {
            if (timer >= maxTimer)
            {
                action.equipment.Equip(action.equipment.PrimaryWeapon);
                return typeof(IdleState);
            }
            return null;
        }
    }

    class MeleeState : State
    {
        float meleeTime;
        float timer;
        bool abort = false;

        public MeleeState(AgentAction action) : base(action) { }

        public override void Before()
        {
            abort = !action.equipment.HasWeaponEquipped;
            meleeTime = action.equipment.CurrentWeaponAttack.MeleeDuration;
            if (!abort)
            {
                timer = 0f;
                action.equipment.CurrentWeaponAttack.MeleeAttack();
            }
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override Type CheckTransitions()
        {
            if (abort || timer >= meleeTime)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

}
