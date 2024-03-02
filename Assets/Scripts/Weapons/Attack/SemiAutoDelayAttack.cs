using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoDelayAttack : RangedWeaponAttack
{
    [SerializeField] float shotDelay = 1f;

    float timer;

    public override void BeginAttack()
    {
        if (timer <= 0)
        {
            if (weaponAmmo.CurrentLoadedAmmo == 0)
            {
                SoundManager.Instance.PlaySoundAtPosition("Dry_Shot", transform.position);
            }
            SpawnProjectile();
            timer = shotDelay;
        }
    }

    public override void DuringAttack()
    {

    }

    public override void EndAttack()
    {

    }

    protected override void Update()
    {
        base.Update();
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }
}
