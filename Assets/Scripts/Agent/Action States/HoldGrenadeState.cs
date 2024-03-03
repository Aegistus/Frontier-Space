using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class HoldGrenadeState
    {
        public HoldGrenadeState(AgentAction action) : base(action) { }

        public override void Before()
        {
            action.currentGrenade = action.equipment.GetGrenade();
            if (action.currentGrenade != null)
            {
                action.equipment.UnEquip(action.equipment.CurrentWeapon);
                action.currentGrenade.gameObject.SetActive(true);
                action.currentGrenade.Arm();
                action.currentGrenade.transform.position = action.eyeTransform.position + .5f * action.eyeTransform.right;
                action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.UpperHoldGrenade);
                action.agentIK.SetHandWeight(Hand.Right, 0);
                action.agentIK.SetHandWeight(Hand.Left, 0);
            }
        }

        public override Type CheckTransitions()
        {
            if (action.currentGrenade == null)
            {
                return typeof(IdleState);
            }
            if (action.controller.ThrowGrenade)
            {
                return typeof(ThrowGrenadeState);
            }
            return null;
        }
    }

}
