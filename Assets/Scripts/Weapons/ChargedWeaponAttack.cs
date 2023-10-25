using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedWeaponAttack : RangedWeaponAttack
{
    [SerializeField] float roundsPerMinute = 120f;
    [SerializeField] float chargeDelay = 2f;

    float chargeTimer = 0f;
    float shotTimer = 0f;
    float shotDelay;

    protected override void Awake()
    {
        base.Awake();
        shotDelay = 60 / roundsPerMinute;
    }

    public override void BeginAttack()
    {
        print("Charging");
        chargeTimer = 0f;
        shotTimer = 0f;
    }

    public override void DuringAttack()
    {
        if (chargeTimer < chargeDelay)
        {
            chargeTimer += Time.deltaTime;
            return;
        }
        shotTimer += Time.deltaTime;
        if (shotTimer >= shotDelay)
        {
            SpawnProjectile();
            shotTimer = 0f;
        }
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
}
