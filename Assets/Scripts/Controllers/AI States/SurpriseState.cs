using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class SurpriseState
    {
        readonly int surpriseAnimationCount = 4;
        float timerMax;
        float timer;

        public SurpriseState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            controller.movement.SwitchState(typeof(AgentMovement.StandState));
            controller.agentAnimator.SetInteger("SurpriseIndex", UnityEngine.Random.Range(0, surpriseAnimationCount));
            controller.agentAnimator.PlayFullBodyAnimation(FullBodyAnimState.Surprised, true);
            timerMax = UnityEngine.Random.Range(controller.reactionTimeMin, controller.reactionTimeMax);
        }

        public override void During()
        {
            timer += Time.deltaTime;
            if (controller.KnownTarget != null)
            {
                controller.LookAt(controller.KnownTarget.position);
            }
        }

        public override Type CheckTransitions()
        {
            if (timer >= timerMax)
            {
                return typeof(AttackingState);
            }
            return null;
        }
    }
}
