using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class GuardingState
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

}
