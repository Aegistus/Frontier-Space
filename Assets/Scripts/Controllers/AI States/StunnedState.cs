using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class StunnedState
    {
        readonly int numOfFlinchAnimations = 5;
        readonly float knockbackTimerMax = .5f;
        float knockbackTimer;
        float timer;
        Vector3 damageDirection;

        public StunnedState(EnemyController controller) : base(controller)
        {
            controller.health.OnDamageTaken += (DamageSource source, float amount, Vector3 direction) => damageDirection = direction;
        }

        public override void Before()
        {
            // pick a random flinch animation
            controller.movement.SetRigWeight(0);
            int randIndex = UnityEngine.Random.Range(0, numOfFlinchAnimations);
            controller.agentAnimator.SetInteger("FlinchIndex", randIndex);
            controller.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.Flinch);
            controller.agentIK.SetHandWeight(Hand.Left, 0);
            controller.Attack = false;
            controller.Forwards = Vector3.Angle(transform.forward, damageDirection) < 45;
            controller.Backwards = Vector3.Angle(-transform.forward, damageDirection) < 45;
            controller.Left = Vector3.Angle(-transform.right, damageDirection) < 45;
            controller.Right = Vector3.Angle(transform.right, damageDirection) < 45;
            timer = 0f;
            knockbackTimer = 0f;
        }

        public override void During()
        {
            timer += Time.deltaTime;
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackTimerMax)
            {
                controller.Forwards = false;
                controller.Backwards = false;
                controller.Right = false;
                controller.Left = false;
            }
        }

        public override void After()
        {
            controller.movement.SetRigWeight(1);
            controller.agentIK.SetHandWeight(Hand.Left, 1);
            controller.Forwards = false;
            controller.Backwards = false;
            controller.Right = false;
            controller.Left = false;
        }

        public override Type CheckTransitions()
        {
            if (timer >= controller.stunDuration)
            {
                return controller.previousStateType;
            }
            return null;
        }
    }
}
