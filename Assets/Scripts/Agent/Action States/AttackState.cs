using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class AttackState
    {
        public AttackState(AgentAction action) : base(action) { }

        public override void Before()
        {
            action.equipment.CurrentWeaponAttack.BeginAttack();
        }

        public override void During()
        {
            action.equipment.CurrentWeaponAttack.DuringAttack();
        }

        public override void After()
        {
            action.equipment.CurrentWeaponAttack.EndAttack();
        }

        public override Type CheckTransitions()
        {
            if (movement.CurrentState == typeof(AgentMovement.RunState))
            {
                return typeof(IdleState);
            }
            if (!action.controller.Attack)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

}
