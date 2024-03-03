using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class MeleeAttackState
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
