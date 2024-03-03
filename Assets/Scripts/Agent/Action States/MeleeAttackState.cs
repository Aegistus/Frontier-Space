using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class MeleeAttackState
    {
        float meleeTime;
        float timer;
        bool abort = false;

        public MeleeAttackState(AgentAction action) : base(action) { }

        public override void Before()
        {
            abort = !action.equipment.HasWeaponEquipped;
            if (!abort)
            {
                meleeTime = action.equipment.CurrentWeaponAttack.MeleeDuration;
                timer = 0f;
                action.equipment.CurrentWeaponAttack.MeleeAttack();
            }
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override Type CheckTransitions()
        {
            if (abort || timer >= meleeTime)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

}

