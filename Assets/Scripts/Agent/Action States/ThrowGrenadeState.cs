using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class ThrowGrenadeState
    {
        float maxTimer = 1f;
        float timer = 0f;

        public ThrowGrenadeState(AgentAction action) : base(action) { }

        public override void Before()
        {
            timer = 0f;
            action.currentGrenade.transform.SetParent(null);
            Rigidbody grenRB = action.currentGrenade.GetComponent<Rigidbody>();
            grenRB.useGravity = true;
            grenRB.AddForce(action.eyeTransform.forward * action.grenadeThrowForce);
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.UpperThrowGrenade);
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override void After()
        {
            action.agentIK.SetHandWeight(Hand.Right, 1);
            action.agentIK.SetHandWeight(Hand.Left, 1);
        }

        public override Type CheckTransitions()
        {
            if (timer >= maxTimer)
            {
                action.equipment.Equip(action.equipment.PrimaryWeapon);
                return typeof(IdleState);
            }
            return null;
        }
    }

}
