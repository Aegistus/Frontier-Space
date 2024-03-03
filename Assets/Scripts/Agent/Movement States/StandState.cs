using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentMovement
{
    public partial class StandState
    {
        readonly float turnAnimationThreshold = .001f;
        Vector3 lastRotation;
        bool turning = false;

        public StandState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Idle, false);
            movement.velocity = Vector3.zero;
            movement.equipment.BobWeapon(false);
            lastRotation = transform.eulerAngles;
        }

        public override void During()
        {
            Vector3 rotationChange = lastRotation - transform.eulerAngles;
            rotationChange *= Time.deltaTime;
            if (rotationChange.y >= turnAnimationThreshold)
            {
                if (!turning)
                {
                    turning = true;
                    movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.TurnRight, false);
                }
            }
            else if (rotationChange.y <= -turnAnimationThreshold)
            {
                if (!turning)
                {
                    turning = true;
                    movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.TurnLeft, false);
                }
            }
            else
            {
                turning = false;
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Idle, false);
            }
            lastRotation = transform.eulerAngles;
        }

        public override Type CheckTransitions()
        {
            if (movement.controller.MovementInput)
            {
                return typeof(WalkState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            if (movement.controller.Crouch)
            {
                return typeof(CrouchState);
            }
            if (!movement.IsGrounded())
            {
                return typeof(FallState);
            }
            return null;
        }
    }

}
