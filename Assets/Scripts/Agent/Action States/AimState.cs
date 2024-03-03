using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class AimState
    {
        public AimState(AgentAction action) : base(action) { }

        public override void Before()
        {
            action.equipment.SetWeaponOffset(WeaponOffset.Aiming);
        }

        public override void After()
        {
            if (movement.CurrentState != typeof(AgentMovement.RunState))
            {
                action.equipment.SetWeaponOffset(WeaponOffset.Idle);
            }
        }

        public override Type CheckTransitions()
        {
            if (action.controller.Attack)
            {
                return typeof(AimAttackState);
            }
            if (movement.CurrentState == typeof(AgentMovement.RunState))
            {
                return typeof(IdleState);
            }
            if (!action.controller.Aim)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

}
