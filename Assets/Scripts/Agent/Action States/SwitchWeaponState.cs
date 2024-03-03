using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class AgentAction
{
    public partial class SwitchWeaponState
    {
        bool success;
        float timer = 0f;

        public SwitchWeaponState(AgentAction action) : base(action) { }

        public override void Before()
        {
            timer = 0f;
            success = action.equipment.TrySwitchWeapon();
            if (success)
            {
                action.equipment.CurrentWeapon.animation.Play("Equip");
            }
        }

        public override void During()
        {
            timer += Time.deltaTime;
        }

        public override void After()
        {
            if (action.equipment.CurrentWeapon != null)
            {
                action.equipment.CurrentWeapon.animation.Stop();
            }
        }

        public override Type CheckTransitions()
        {
            if (!success)
            {
                return typeof(IdleState);
            }
            if (timer >= action.switchWeaponTime)
            {
                return typeof(IdleState);
            }
            return null;
        }
    }

}
