using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentMovement
{
    public partial class CrouchState
    {
        public CrouchState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            movement.equipment.SetWeaponOffset(WeaponOffset.Crouching);
        }

        public override void DuringPhysics()
        {
            Vector3 input = transform.TransformDirection(movement.controller.GetMovementInput());
            movement.Move(input, movement.crouchSpeed);
        }

        public override void During()
        {
            if (movement.controller.Forwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchForwards, false);
            }
            else if (movement.controller.Backwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchBackwards, false);
            }
            else if (movement.controller.Left)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchLeft, false);
            }
            else if (movement.controller.Right)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchRight, false);
            }
            else
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.CrouchIdle, false);
            }
        }

        public override void After()
        {
            movement.equipment.SetWeaponOffset(WeaponOffset.Idle);
        }

        public override Type CheckTransitions()
        {
            if (!movement.controller.Crouch)
            {
                return typeof(StandState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            return null;
        }
    }
}
