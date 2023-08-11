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

    public Transform AttackTarget { get; private set; }

    State currentState;
    Dictionary<Type, State> availableStates;
    NavMeshAgent navAgent;
    Queue<Transform> patrolNodeQueue;

    readonly float destinationTolerance = .1f; // how far away is considered "arrived at destination"
    Vector3 heightOffset = Vector3.up;
    Vector3 lookTargetDefaultPos = new Vector3(0, 1, 10);

    PlayerController player;
    FieldOfView fov;

    private void Awake()
    {
        LookTarget = lookTarget;
        navAgent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        fov.OnPlayerFound += Fov_OnTargetFound;
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
        };
        currentState = availableStates[typeof(GuardingState)];
    }

    private void Fov_OnTargetFound(Transform target)
    {
        AttackTarget = target;
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
    }


    private void OnDrawGizmos()
    {
        if (navAgent && navAgent.hasPath)
        {
            print("Drawing");
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
        public AttackingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Player detected");
        }

        public override void During()
        {
            controller.LookTarget.position = controller.AttackTarget.position;
            controller.Attack = true;
        }
    }
}
