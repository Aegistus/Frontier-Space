using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class AimingState
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
}
