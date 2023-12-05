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