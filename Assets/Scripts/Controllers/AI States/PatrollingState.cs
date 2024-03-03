using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class PatrollingState
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
}