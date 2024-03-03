using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentMovement
{
    public partial class FallState 
    {
        Vector3 initialDirection;
        float initialSpeed;

        public FallState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Fall, false);
            initialDirection = movement.velocity;
            initialSpeed = initialDirection.magnitude;
        }

        public override void DuringPhysics()
        {
            Vector3 input = transform.TransformDirection(movement.controller.GetMovementInput());
            movement.Move(input, movement.airMoveSpeed);
            movement.Move(initialDirection, initialSpeed);
        }

        public override void After()
        {
            SoundManager.Instance.PlaySoundAtPosition("Footstep_Run", transform.position);
        }

        public override Type CheckTransitions()
        {
            if (movement.IsGrounded())
            {
                return typeof(StandState);
            }
            return null;
        }
    }
}
