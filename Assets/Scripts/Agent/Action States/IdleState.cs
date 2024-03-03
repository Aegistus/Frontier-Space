using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class IdleState
    {
        public IdleState(AgentAction action) : base(action) { }

        public override void Before()
        {
            action.agentAnimator.PlayUpperBodyAnimation(UpperBodyAnimState.None);
        }

        public override Type CheckTransitions()
        {
            if (action.equipment.HasWeaponEquipped)
            {
                if (movement.CurrentState != typeof(AgentMovement.RunState))
                {
                    if (action.controller.Attack)
                    {
                        return typeof(AttackState);
                    }
                    if (action.controller.Aim)
                    {
                        return typeof(AimState);
                    }
                }
                if (action.controller.Reload)
                {
                    if (action.equipment.CurrentWeaponAmmunition.CurrentCarriedAmmo > 0 || action.equipment.CurrentWeaponAmmunition.InfiniteCarriedAmmo)
                    {
                        return typeof(ReloadState);
                    }
                }
            }
            if (action.controller.Interact)
            {
                return typeof(InteractState);
            }
            if (action.controller.SwitchWeapon)
            {
                return typeof(SwitchWeaponState);
            }
            if (action.controller.ArmGrenade)
            {
                return typeof(HoldGrenadeState);
            }
            if (action.controller.Melee)
            {
                return typeof(MeleeAttackState);
            }
            return null;
        }
    }

}
