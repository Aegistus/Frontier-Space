using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentMovement
{
    public partial class WalkState 
    {
        public WalkState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            movement.equipment.BobWeapon(true);
        }

        public override void DuringPhysics()
        {
            movement.Move(movement.controller.GetMovementInput(), movement.walkSpeed);
        }

        public override void During()
        {
            if (movement.controller.Forwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkForwards, false);
            }
            else if (movement.controller.Backwards)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkBackwards, false);
            }
            else if (movement.controller.Left)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkLeft, false);
            }
            else if (movement.controller.Right)
            {
                movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.WalkRight, false);
            }
        }

        public override Type CheckTransitions()
        {
            if (movement.controller.NoMovementInput)
            {
                return typeof(StandState);
            }
            if (movement.controller.Jump)
            {
                return typeof(JumpState);
            }
            if (movement.controller.Run)
            {
                return typeof(RunState);
            }
            if (movement.controller.Crouch)
            {
                return typeof(CrouchState);
            }
            return null;
        }
    }

}
