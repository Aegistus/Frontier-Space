using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class AttackingState
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

}
