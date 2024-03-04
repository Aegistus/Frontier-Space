using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public partial class EnemyController : AgentController
{
    [SerializeField] Transform lookTarget;
    [SerializeField] Transform[] patrolNodes;
    [SerializeField] bool patrolling;
    [SerializeField] bool onGuard = false;
    [SerializeField] bool holdPosition = false;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float reactionTimeMin = .5f;
    [SerializeField] float reactionTimeMax = 1f;
    [Header("Ranged Combat")]
    [SerializeField] Vector3 aimOffset = new Vector3(0, 1, 0);
    [SerializeField] float attackBurstTime = 2f;
    [SerializeField] float attackWaitTime = 1f;
    [SerializeField] float suppressionDuration = 2f;
    [Range(0f, 1f)]
    [SerializeField] float crouchWhileAttackingChance = .5f;
    [Header("Melee Combat")]
    [SerializeField] float meleeAttackRange = 2f;
    [SerializeField] float meleeAttackCooldown = 3f;
    [Header("Stun")]
    [SerializeField] float stunDamageThreshold = 10;
    [SerializeField] float stunDuration = 2f;
    [Header("Ammo")]
    [SerializeField] float minCarriedAmmoPercent = .3f;
    [SerializeField] float maxCarriedAmmoPercent = .6f;

    public Transform VisibleTarget => fov.visibleTargets.Count > 0 ? fov.visibleTargets[0] : null;
    public Transform KnownTarget => fov.knownTargets.Count > 0 ? fov.knownTargets[0] : null;

    State currentState;
    Type previousStateType;
    Dictionary<Type, State> availableStates;
    NavMeshAgent navAgent;
    Queue<Transform> patrolNodeQueue;

    readonly float destinationTolerance = .1f; // how far away is considered "arrived at destination"
    Vector3 heightOffset = new Vector3(0, 1.6f, 0);
    Vector3 lookTargetDefaultPos = new Vector3(0, 1, 10);
    float meleeCooldownTimer;
    FieldOfView fov;
    AgentEquipment equipment;
    AgentMovement movement;
    HumanoidAnimator agentAnimator;
    HumanoidIK agentIK;
    AgentAction action;
    AgentHealth health;

    private void Awake()
    {
        LookTarget = lookTarget;
        navAgent = GetComponent<NavMeshAgent>();
        fov = GetComponentInChildren<FieldOfView>();
        equipment = GetComponent<AgentEquipment>();
        movement = GetComponent<AgentMovement>();
        agentAnimator = GetComponentInChildren<HumanoidAnimator>();
        agentIK = GetComponentInChildren<HumanoidIK>();
        action = GetComponentInChildren<AgentAction>();
        health = GetComponent<AgentHealth>();
        health.OnAgentDeath += OnDeath;
        health.OnDamageTaken += CheckForStun;
        if (patrolNodes.Length == 0)
        {
            patrolling = false;
        }
        patrolNodeQueue = new Queue<Transform>();
        for (int i = 0; i < patrolNodes.Length; i++)
        {
            patrolNodeQueue.Enqueue(patrolNodes[i]);
        }
        availableStates = new Dictionary<Type, State>()
        {
            { typeof(GuardingState), new GuardingState(this) },
            { typeof(PatrollingState), new PatrollingState(this) },
            { typeof(AttackingState), new AttackingState(this) },
            { typeof(AimingState), new AimingState(this) },
            { typeof(ReloadingState), new ReloadingState(this) },
            { typeof(ChasingState), new ChasingState(this) },
            { typeof(SuppressingState), new SuppressingState(this) },
            { typeof(StunnedState), new StunnedState(this) },
            { typeof(SurpriseState), new SurpriseState(this) },
            { typeof(MeleeAttackState), new MeleeAttackState(this) },
        };
        currentState = availableStates[typeof(GuardingState)];
    }

    private void Start()
    {
        float percentCarriedAmmo = UnityEngine.Random.Range(minCarriedAmmoPercent, maxCarriedAmmoPercent);
        if (equipment.PrimaryWeapon != null)
        {
            equipment.PrimaryWeapon.ammo.InfiniteCarriedAmmo = true;
            equipment.PrimaryWeapon.ammo.AddAmmo((int)(equipment.PrimaryWeapon.ammo.MaxCarriedAmmo * percentCarriedAmmo));
        }
        if (equipment.SecondaryWeapon != null)
        {
            equipment.SecondaryWeapon.ammo.InfiniteCarriedAmmo = true;
            equipment.SecondaryWeapon.ammo.AddAmmo((int)(equipment.SecondaryWeapon.ammo.MaxCarriedAmmo * percentCarriedAmmo));
        }
    }

    private void Update()
    {
        currentState.During();
        Type nextState = currentState.CheckTransitions();
        if (nextState != null)
        {
            ChangeState(nextState);
        }
        // update body rotation
        Quaternion currentRotation = transform.rotation;
        transform.LookAt(LookTarget);
        Quaternion targetRotation = transform.rotation;
        transform.rotation = currentRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        if (meleeCooldownTimer > 0)
        {
            meleeCooldownTimer -= Time.deltaTime;
        }
    }

    void ChangeState(Type nextState)
    {
        currentState.After();
        previousStateType = currentState.GetType();
        currentState = availableStates[nextState];
        currentState.Before();
    }

    void OnDeath()
    {
        GetComponent<AgentMovement>().enabled = false;
        fov.enabled = false;
        if (equipment.PrimaryWeapon != null)
        {
            equipment.PrimaryWeapon.ammo.InfiniteCarriedAmmo = false;
        }
        if (equipment.SecondaryWeapon != null)
        {
            equipment.SecondaryWeapon.ammo.InfiniteCarriedAmmo = false;
        }
        equipment.DropWeapon();
        // do this last:
        enabled = false;
    }

    void LookAt(Vector3 position)
    {
        if (movement.CurrentState != typeof(AgentMovement.CrouchState))
        {
            position += aimOffset;
        }
        lookTarget.position = position;
    }

    void MoveToDestination(bool run = false)
    {
        if (!navAgent.hasPath)
        {
            return;
        }
        Forwards = true;
        Run = run;
        if (navAgent.path.corners.Length > 1)
        {
            Vector3 lookOffset = (navAgent.path.corners[1] - transform.position).normalized;
            LookAt(navAgent.path.corners[1] + heightOffset + lookOffset);
        }
    }

    private void CheckForStun(DamageSource source, float damage, Vector3 direction)
    {
        if (damage >= stunDamageThreshold && currentState.GetType() != typeof(StunnedState))
        {
            ChangeState(typeof(StunnedState));
        }
    } 

    private void OnDrawGizmos()
    {
        if (navAgent && navAgent.hasPath)
        {
            for (int i = 0; i < navAgent.path.corners.Length; i++)
            {
                Gizmos.DrawSphere(navAgent.path.corners[0], .4f);
            }
        }
    }

    public abstract class State
    {
        protected EnemyController controller;
        protected GameObject gameObject;
        protected Transform transform;
        protected NavMeshAgent navAgent;

        public State(EnemyController controller)
        {
            this.controller = controller;
            gameObject = controller.gameObject;
            transform = controller.transform;
            navAgent = controller.navAgent;
        }

        public virtual void Before() { }
        public virtual void During() { }
        public virtual void DuringPhysics() { }
        public virtual void After() { }
        public virtual Type CheckTransitions() { return null; }
    }

    public partial class GuardingState : State { }

    public partial class PatrollingState : State { }

    public partial class AttackingState : State { }

    public partial class AimingState : State { }

    public partial class ReloadingState : State { }

    public partial class ChasingState : State { }

    public partial class SuppressingState : State { }

    public partial class StunnedState : State { }

    public partial class SurpriseState : State { }

    public partial class MeleeAttackState : State { }
}
