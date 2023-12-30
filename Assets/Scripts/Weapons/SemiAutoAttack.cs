using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoAttack : RangedWeaponAttack
{

    public override void BeginAttack()
    {
        SpawnProjectile();
    }

    public override void DuringAttack()
    {

    }

    public override void EndAttack()
    {

    }
}
