using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class ReloadState
    {
        bool successful;

        public ReloadState(AgentAction action) : base(action) { }

        public override void Before()
        {
            successful = action.equipment.CurrentWeaponAmmunition.TryReload();
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.UpperReload);
            //action.agentIK.SetHandWeight(Hand.Left, 0);
            action.equipment.SetWeaponOffset(WeaponOffset.Reloading);
        }

        public override void After()
        {
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.None);
            //action.agentIK.SetHandWeight(Hand.Left, 1);
            action.equipment.SetWeaponOffset(WeaponOffset.Idle);
        }

        public override Type CheckTransitions()
        {
            if (!successful || !action.equipment.CurrentWeaponAmmunition.Reloading)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

}
