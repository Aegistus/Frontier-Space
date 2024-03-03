using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentMovement
{
    public partial class JumpState 
    {
        float timer = 0f;
        float fallDelay = .5f;
        Vector3 initialDirection;
        float initialSpeed;

        public JumpState(AgentMovement movement) : base(movement) { }

        public override void Before()
        {
            timer = 0f;
            movement.verticalVelocity += movement.jumpVelocity;
            movement.humanoidAnimator.PlayFullBodyAnimation(FullBodyAnimState.Jump, false);
            initialDirection = movement.velocity;
            initialSpeed = initialDirection.magnitude;
            SoundManager.Instance.PlaySoundAtPosition("Jump", transform.position);
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override void DuringPhysics()
        {
            movement.Move(initialDirection, initialSpeed);
        }

        public override Type CheckTransitions()
        {
            if (timer >= fallDelay)
            {
                return typeof(FallState);
            }
            return null;
        }
    }

}
