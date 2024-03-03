using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class EnemyController
{
    public partial class ReloadingState
    {
        public ReloadingState(EnemyController controller) : base(controller) { }

        public override void Before()
        {
            print("Reloading State");
            controller.Reload = true;
        }

        public override void After()
        {
            controller.Reload = false;
        }

        public override Type CheckTransitions()
        {
            if (!controller.equipment.CurrentWeaponAmmunition.Reloading)
            {
                return typeof(AttackingState);
            }
            if (controller.VisibleTarget == null && controller.KnownTarget != null)
            {
                return typeof(ChasingState);
            }
            return null;
        }
    }
}
