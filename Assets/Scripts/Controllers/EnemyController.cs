using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : AgentController
{
    [SerializeField] Transform[] patrolNodes;
    [SerializeField] bool patrolling;

    State currentState;
    Dictionary<Type, State> availableStates;
    NavMeshAgent navAgent;
    Queue<Transform> patrolNodeQueue;

    readonly float destinationTolerance = .1f; // how far away is considered "arrived at destination"
    Vector3 heightOffset = Vector3.up;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        patrolNodeQueue = new Queue<Transform>();
        for (int i = 0; i < patrolNodes.Length; i++)
        {
            patrolNodeQueue.Enqueue(patrolNodes[i]);
        }
        availableStates = new Dictionary<Type, State>()
        {
            { typeof(GuardingState), new GuardingState(this) },
            { typeof(PatrollingState), new PatrollingState(this) },
        };
        currentState = availableStates[typeof(GuardingState)];

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
        //transform.LookAt(Target);
        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public override void FindNewTarget()
    {
        
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

        public override Type CheckTransitions()
        {
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
        Vector3 nextPoint;

        public PatrollingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            currentNode = controller.patrolNodeQueue.Dequeue();
        }

        public override void During()
        {
            controller.Forwards = true;
            transform.LookAt(currentNode.position + controller.heightOffset);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            if (Vector3.Distance(transform.position, currentNode.position) <= controller.destinationTolerance)
            {
                controller.patrolNodeQueue.Enqueue(currentNode);
                currentNode = controller.patrolNodeQueue.Dequeue();
            }
        }

    }
}
