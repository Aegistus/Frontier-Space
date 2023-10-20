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

    void SpawnProjectile()
    {
        if (weaponAmmo.TryUseAmmo())
        {
            GameObject projectile = PoolManager.Instance.SpawnObjectWithLifetime(projectileID, projectileSpawnPoint.position, projectileSpawnPoint.rotation, 10f);
            float damage = Random.Range(damageMin, damageMax);
            projectile.GetComponent<Projectile>().SetDamage(damage, Source);
            ApplyRecoil();
            SoundManager.Instance.PlaySoundAtPosition(shootSoundID, projectileSpawnPoint.position);
        }
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
