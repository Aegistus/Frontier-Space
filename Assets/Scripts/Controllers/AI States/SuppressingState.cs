using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class SuppressingState
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
}
