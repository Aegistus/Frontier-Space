using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : AgentController
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
        if (equipment.PrimaryWeapon != null)
        {
            equipment.PrimaryWeapon.ammo.InfiniteCarriedAmmo = true;
        }
        if (equipment.SecondaryWeapon != null)
        {
            equipment.SecondaryWeapon.ammo.InfiniteCarriedAmmo = true;
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

    abstract class State
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

    class GuardingState : State
    {
        public GuardingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Guarding");
            controller.LookTarget.localPosition = controller.lookTargetDefaultPos;
            controller.Forwards = false;
            controller.Run = false;
            controller.Attack = false;
        }

        public override Type CheckTransitions()
        {
            if (controller.VisibleTarget != null)
            {
                if (!controller.onGuard)
                {
                    return typeof(SurpriseState);
                }
                return typeof(AttackingState);
            }
            if (!controller.holdPosition && controller.VisibleTarget == null && controller.KnownTarget != null)
            {
                return typeof(ChasingState);
            }
            if (controller.patrolling)
            {
                return typeof(PatrollingState);
            }
            return null;
        }
    }

    class PatrollingState : State
    {
        Transform currentNode;

        public PatrollingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            currentNode = controller.patrolNodeQueue.Dequeue();
            navAgent.SetDestination(currentNode.position);
            controller.Run = false;
        }

        public override void During()
        {
            controller.MoveToDestination();
            if (Vector3.Distance(transform.position, currentNode.position) <= controller.destinationTolerance)
            {
                controller.patrolNodeQueue.Enqueue(currentNode);
                currentNode = controller.patrolNodeQueue.Dequeue();
                navAgent.SetDestination(currentNode.position);
            }
            controller.LookAt(currentNode.position);
        }

        public override void After()
        {
            if (currentNode != null)
            {
                controller.patrolNodeQueue.Enqueue(currentNode);
            }
            controller.Forwards = false;
        }

        public override Type CheckTransitions()
        {
            if (controller.VisibleTarget != null)
            {
                if (!controller.onGuard)
                {
                    return typeof(SurpriseState);
                }
                return typeof(AttackingState);
            }
            if (!controller.holdPosition && controller.VisibleTarget == null && controller.KnownTarget != null)
            {
                return typeof(ChasingState);
            }
            return null;
        }
    }

    class AttackingState : State
    {
        float attackTimer;
        float crouchChance;

        public AttackingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Attacking");
            attackTimer = controller.attackBurstTime;
            controller.Forwards = false;
            crouchChance = UnityEngine.Random.Range(0f, 1f);
        }

        public override void During()
        {
            if (controller.VisibleTarget != null)
            {
                controller.LookAt(controller.VisibleTarget.position);
            }
            if (crouchChance < controller.crouchWhileAttackingChance)
            {
                controller.Crouch = true;
            }
            else
            {
                controller.Crouch = false;
            }
            controller.Attack = true;
            attackTimer -= Time.deltaTime;
        }

        public override Type CheckTransitions()
        {
            if (controller.VisibleTarget == null && controller.KnownTarget != null)
            {
                return typeof(SuppressingState);
            }
            if (controller.meleeCooldownTimer <= 0 && controller.VisibleTarget != null && Vector3.Distance(controller.VisibleTarget.position, transform.position) < controller.meleeAttackRange)
            {
                controller.Attack = false;
                return typeof(MeleeAttackState);
            }
            if (controller.equipment.CurrentWeaponAmmunition.CurrentLoadedAmmo == 0)
            {
                controller.Attack = false;
                return typeof(ReloadingState);
            }
            if (controller.VisibleTarget == null && controller.KnownTarget == null)
            {
                controller.Attack = false;
                return typeof(GuardingState);
            }
            if (attackTimer <= 0)
            {
                controller.Attack = false;
                return typeof(AimingState);
            }
            return null;
        }
    }

    class AimingState : State
    {
        float waitTimer;
        bool strafeLeft;

        public AimingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            waitTimer = controller.attackWaitTime;
            controller.Forwards = false;
            controller.Run = false;
            strafeLeft = UnityEngine.Random.value > .5;
        }

        public override void During()
        {
            waitTimer -= Time.deltaTime;
            if (controller.VisibleTarget != null)
            {
                controller.LookAt(controller.VisibleTarget.position);
                if (!controller.Crouch && !controller.holdPosition)
                {
                    if (strafeLeft)
                    {
                        controller.Left = true;
                    }
                    else
                    {
                        controller.Right = true;
                    }
                }
            }
        }

        public override void After()
        {
            controller.Left = false;
            controller.Right = false;
        }

        public override Type CheckTransitions()
        {
            if (controller.VisibleTarget == null && controller.KnownTarget != null)
            {
                return typeof(SuppressingState);
            }
            if (controller.meleeCooldownTimer <= 0 && controller.VisibleTarget != null && Vector3.Distance(controller.VisibleTarget.position, transform.position) < controller.meleeAttackRange)
            {
                controller.Attack = false;
                return typeof(MeleeAttackState);
            }
            if (waitTimer <= 0)
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }

    class ReloadingState : State
    {
        public ReloadingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Reloading State");
            controller.Reload = true;
        }

        public override void After()
        {
            controller.Reload = false;
        }

        public override Type CheckTransitions()
        {
            if (!controller.equipment.CurrentWeaponAmmunition.Reloading)
            {
                return typeof(AttackingState);
            }
            if (controller.VisibleTarget == null && controller.KnownTarget != null)
            {
                return typeof(ChasingState);
            }
            return null;
        }
    }

    class ChasingState : State
    {
        float runDistance = 20; // how far away the target needs to be before the enemy will start running rather than walking.
        float reactionTimer;

        public ChasingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Chasing");
            navAgent.SetDestination(controller.KnownTarget.position);
            reactionTimer = UnityEngine.Random.Range(controller.reactionTimeMin, controller.reactionTimeMax);
            controller.Attack = false;
            controller.Crouch = false;
        }

        public override void During()
        {
            bool run = false;
            if (controller.KnownTarget != null)
            {
                navAgent.SetDestination(controller.KnownTarget.position);
                run = Vector3.Distance(transform.position, controller.KnownTarget.position) > runDistance;
            }
            controller.MoveToDestination(run);
            if (controller.VisibleTarget)
            {
                reactionTimer -= Time.deltaTime;
            }
        }

        public override Type CheckTransitions()
        {
            if (controller.KnownTarget == null || controller.holdPosition)
            {
                return typeof(GuardingState);
            }
            if (controller.VisibleTarget != null && reactionTimer <= 0)
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }

    class SuppressingState : State
    {
        float suppressionTimer;
        float attackTimer;
        float waitTimer;
        bool currentlyAttacking;

        // kept so that the enemy doesn't forget about their target by the end of suppression.
        Transform suppressingTarget;
        Vector3 aimPosition;

        public SuppressingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            suppressionTimer = controller.suppressionDuration;
            suppressingTarget = controller.KnownTarget;
            aimPosition = suppressingTarget.position;
            attackTimer = controller.attackBurstTime;
            waitTimer = 0;
            currentlyAttacking = true;
        }

        public override void During()
        {
            suppressionTimer -= Time.deltaTime;
            controller.LookAt(aimPosition);
            if (attackTimer > 0)
            {
                controller.Attack = true;
                attackTimer -= Time.deltaTime;
            }
            else if (currentlyAttacking)
            {
                currentlyAttacking = false;
                controller.Attack = false;
                waitTimer = controller.attackWaitTime;
            }
            if (waitTimer > 0)
            {
                controller.Attack = false;
                waitTimer -= Time.deltaTime;
            }
            else if (!currentlyAttacking)
            {
                currentlyAttacking = true;
                attackTimer = controller.attackBurstTime;
            }
        }

        public override Type CheckTransitions()
        {
            if (suppressionTimer <= 0)
            {
                if (controller.holdPosition)
                {
                    return typeof(GuardingState);
                }
                else
                {
                    controller.fov.AddKnownTarget(suppressingTarget);
                    return typeof(ChasingState);
                }
            }
            if (controller.equipment.CurrentWeaponAmmunition.CurrentLoadedAmmo == 0)
            {
                return typeof(ReloadingState);
            }
            if (controller.VisibleTarget != null)
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }

    class StunnedState : State
    {
        readonly int numOfFlinchAnimations = 5;
        readonly float knockbackTimerMax = .5f;
        float knockbackTimer;
        float timer;
        Vector3 damageDirection;

        public StunnedState(EnemyController controller) : base(controller)
        {
            controller.health.OnDamageTaken += (DamageSource source, float amount, Vector3 direction) => damageDirection = direction;
        }

        public override void Before()
        {
            // pick a random flinch animation
            controller.movement.SetRigWeight(0);
            int randIndex = UnityEngine.Random.Range(0, numOfFlinchAnimations);
            controller.agentAnimator.SetInteger("FlinchIndex", randIndex);
            controller.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.Flinch);
            controller.agentIK.SetHandWeight(Hand.Left, 0);
            controller.Attack = false;
            controller.Forwards = Vector3.Angle(transform.forward, damageDirection) < 45;
            controller.Backwards = Vector3.Angle(-transform.forward, damageDirection) < 45;
            controller.Left = Vector3.Angle(-transform.right, damageDirection) < 45;
            controller.Right = Vector3.Angle(transform.right, damageDirection) < 45;
            timer = 0f;
            knockbackTimer = 0f;
        }

        public override void During()
        {
            timer += Time.deltaTime;
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackTimerMax)
            {
                controller.Forwards = false;
                controller.Backwards = false;
                controller.Right = false;
                controller.Left = false;
            }
        }

        public override void After()
        {
            controller.movement.SetRigWeight(1);
            controller.agentIK.SetHandWeight(Hand.Left, 1);
            controller.Forwards = false;
            controller.Backwards = false;
            controller.Right = false;
            controller.Left = false;
        }

        public override Type CheckTransitions()
        {
            if (timer >= controller.stunDuration)
            {
                return controller.previousStateType;
            }
            return null;
        }
    }

    class SurpriseState : State
    {
        readonly int surpriseAnimationCount = 4;
        float timerMax;
        float timer;

        public SurpriseState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            controller.movement.SwitchState(typeof(AgentMovement.StandState));
            controller.agentAnimator.SetInteger("SurpriseIndex", UnityEngine.Random.Range(0, surpriseAnimationCount));
            controller.agentAnimator.PlayFullBodyAnimation(FullBodyAnimState.Surprised, true);
            timerMax = UnityEngine.Random.Range(controller.reactionTimeMin, controller.reactionTimeMax);
        }

        public override void During()
        {
            timer += Time.deltaTime;
            if (controller.KnownTarget != null)
            {
                controller.LookAt(controller.KnownTarget.position);
            }
        }

        public override Type CheckTransitions()
        {
            if (timer >= timerMax)
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }

    class MeleeAttackState : State
    {
        bool meleeStarted = false;
        float walkForwardDuration = .5f;
        float walkForwardTimer;

        public MeleeAttackState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            controller.Melee = true;
            controller.Forwards = true;
            controller.meleeCooldownTimer = controller.meleeAttackCooldown;
            walkForwardTimer = walkForwardDuration;
        }

        public override void During()
        {
            if (!meleeStarted && controller.action.CurrentState == typeof(AgentAction.MeleeState))
            {
                meleeStarted = true;
            }
            controller.LookAt(controller.VisibleTarget.position);
            controller.Forwards = walkForwardTimer > 0;
            walkForwardTimer -= Time.deltaTime;
        }

        public override void After()
        {
            controller.Melee = false;
            controller.Forwards = false;
        }

        public override Type CheckTransitions()
        {
            if (meleeStarted && controller.action.CurrentState != typeof(AgentAction.MeleeState))
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }
}
