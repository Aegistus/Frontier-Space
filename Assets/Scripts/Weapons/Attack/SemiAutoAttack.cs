using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoAttack : RangedWeaponAttack
{

    public override void BeginAttack()
    {
        if (weaponAmmo.CurrentLoadedAmmo == 0)
        {
            SoundManager.Instance.PlaySoundAtPosition("Dry_Shot", transform.position);
        }
        SpawnProjectile();
    }

    public override void DuringAttack()
    {

    }

    public override void EndAttack()
    {

    }
}
