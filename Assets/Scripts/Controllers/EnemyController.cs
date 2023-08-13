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
    [SerializeField] float reactionTimeMin = .5f;
    [SerializeField] float reactionTimeMax = 1f;
    [SerializeField] float attackBurstTime = 2f;
    [SerializeField] float attackWaitTime = 1f;
    [Range(0f, 1f)]
    [SerializeField] float crouchWhileAttackingChance = .5f;

    public Transform AttackTarget => fov.visibleTargets.Count > 0 ? fov.visibleTargets[0] : null;

    State currentState;
    Dictionary<Type, State> availableStates;
    NavMeshAgent navAgent;
    Queue<Transform> patrolNodeQueue;

    readonly float destinationTolerance = .1f; // how far away is considered "arrived at destination"
    Vector3 heightOffset = Vector3.up;
    Vector3 lookTargetDefaultPos = new Vector3(0, 1, 10);
    Vector3 playerLastLocation;

    FieldOfView fov;
    AgentEquipment equipment;

    private void Awake()
    {
        LookTarget = lookTarget;
        navAgent = GetComponent<NavMeshAgent>();
        fov = GetComponentInChildren<FieldOfView>();
        equipment = GetComponent<AgentEquipment>();
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
        };
        currentState = availableStates[typeof(GuardingState)];
        GetComponent<AgentHealth>().OnAgentDeath += () => enabled = false;
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
        transform.LookAt(LookTarget);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        if (AttackTarget != null)
        {
            playerLastLocation = AttackTarget.position;
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
            controller.LookTarget.localPosition = controller.lookTargetDefaultPos;
        }

        public override Type CheckTransitions()
        {
            if (controller.AttackTarget != null)
            {
                return typeof(AttackingState);
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
            controller.LookTarget.localPosition = controller.lookTargetDefaultPos;
            currentNode = controller.patrolNodeQueue.Dequeue();
            navAgent.SetDestination(currentNode.position);
        }

        public override void During()
        {
            controller.Forwards = true;
            transform.LookAt(navAgent.path.corners[1] + controller.heightOffset);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            if (Vector3.Distance(transform.position, currentNode.position) <= controller.destinationTolerance)
            {
                controller.patrolNodeQueue.Enqueue(currentNode);
                currentNode = controller.patrolNodeQueue.Dequeue();
                navAgent.SetDestination(currentNode.position);
            }
        }

        public override void After()
        {
            controller.Forwards = false;
        }

        public override Type CheckTransitions()
        {
            if (controller.AttackTarget != null)
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }

    class AttackingState : State
    {
        float attackTimer;
        float reactionTimer;
        float crouchChance;

        public AttackingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Player detected");
            attackTimer = controller.attackBurstTime;
            reactionTimer = UnityEngine.Random.Range(controller.reactionTimeMin, controller.reactionTimeMax);
            float crouchChance = UnityEngine.Random.Range(0f, 1f);

        }

        public override void During()
        {
            if (reactionTimer > 0)
            {
                reactionTimer -= Time.deltaTime;
                return;
            }
            if (crouchChance <= controller.crouchWhileAttackingChance)
            {
                controller.Crouch = true;
            }
            if (controller.AttackTarget != null)
            {
                controller.LookTarget.position = controller.AttackTarget.position;
            }
            controller.Attack = true;
            attackTimer -= Time.deltaTime;
        }

        public override void After()
        {
            controller.Attack = false;
        }

        public override Type CheckTransitions()
        {
            if (controller.equipment.CurrentWeaponAmmunition.CurrentLoadedAmmo == 0)
            {
                return typeof(ReloadingState);
            }
            if (controller.AttackTarget == null)
            {
                return typeof(ChasingState);
            }
            if (attackTimer <= 0)
            {
                return typeof(AimingState);
            }
            return null;
        }
    }

    class AimingState : State
    {
        float waitTimer;

        public AimingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            waitTimer = controller.attackWaitTime;
        }

        public override void During()
        {
            waitTimer -= Time.deltaTime;
        }

        public override Type CheckTransitions()
        {
            if (controller.AttackTarget == null)
            {
                return typeof(ChasingState);
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
            return null;
        }
    }

    class ChasingState : State
    {
        public ChasingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Chasing");
            navAgent.SetDestination(controller.playerLastLocation);
            controller.Attack = false;
        }

        public override void During()
        {
            controller.Forwards = true;
            transform.LookAt(navAgent.path.corners[0] + controller.heightOffset);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        public override Type CheckTransitions()
        {
            if (controller.AttackTarget != null)
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }
}
