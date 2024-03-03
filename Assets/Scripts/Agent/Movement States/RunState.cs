using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentMovement
{
    public partial class RunState
    {
        public RunState(AgentMovement movement) : base(movement) { }

        public override void DuringPhysics()
        {
            movement.Move(movement.controller.GetMovementInput(), movement.runSpeed);
        }

        public override void Before()
        {
            movement.humanoidAnimator.SetRigWeight(.5f);
            movement.equipment.SetWeaponOffset(WeaponOffset.Running);
            movement.equipment.BobWeapon(true);
        }

        public override void During()
        {
            if (movement.controller.Left)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.RunLeft, false);
            }
            else if (movement.controller.Right)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.RunRight, false);
            }
            else
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.RunForwards, false);
            }
        }

        public override void After()
        {
            movement.humanoidAnimator.SetRigWeight(1f);
            movement.equipment.SetWeaponOffset(WeaponOffset.Idle);
        }

        public override Type CheckTransitions()
        {
            if (movement.controller.NoMovementInput)
            {
                return typeof(StandState);
            }
            if (!movement.controller.Run)
            {
                return typeof(WalkState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            return null;
        }
    }
}
