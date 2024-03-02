using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullAutoAttack : RangedWeaponAttack
{
    [SerializeField] float roundsPerMinute = 120f;
    float shotDelay;
    float timer = 0f;

    protected override void Awake()
    {
        base.Awake();
        shotDelay = 60 / roundsPerMinute;
    }

    public override void BeginAttack()
    {
        if (weaponAmmo.CurrentLoadedAmmo == 0)
        {
            SoundManager.Instance.PlaySoundAtPosition("Dry_Shot", transform.position);
        }
        SpawnProjectile();
        timer = 0f;
    }

    public override void DuringAttack()
    {
        timer += Time.deltaTime;
        if (timer >= shotDelay)
        {
            SpawnProjectile();
            timer = 0f;
        }
    }

    public override void EndAttack()
    {

    }
}
