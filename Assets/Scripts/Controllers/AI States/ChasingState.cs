using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class ChasingState
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
}
