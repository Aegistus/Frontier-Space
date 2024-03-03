using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class AimAttackState
    {
        public AimAttackState(AgentAction action) : base(action) { }

        public override void Before()
        {
            if (action.equipment.CurrentWeaponAttack is RangedWeaponAttack attack)
            {
                attack.Aimed = true;
            }
            action.equipment.CurrentWeaponAttack.BeginAttack();
            action.equipment.SetWeaponOffset(WeaponOffset.Aiming);
        }

        public override void During()
        {
            action.equipment.CurrentWeaponAttack.DuringAttack();
        }

        public override void After()
        {
            action.equipment.CurrentWeaponAttack.EndAttack();
            if (action.equipment.CurrentWeaponAttack is RangedWeaponAttack attack)
            {
                attack.Aimed = false;
            }
        }

        public override Type CheckTransitions()
        {
            if (!action.controller.Attack)
            {
                return typeof(AimState);
            }
            return null;
        }
    }

}
