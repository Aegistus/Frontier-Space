using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifleAttack : RangedWeaponAttack
{
    [SerializeField] float roundsPerMinute = 120f;

    float shotDelay;
    float timer = 0f;

    private void Awake()
    {
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
        GameObject projectile = PoolManager.Instance.SpawnObjectWithLifetime(projectileID, projectileSpawnPoint.position, projectileSpawnPoint.rotation, 10f);
        float damage = Random.Range(damageMin, damageMax);
        projectile.GetComponent<Projectile>().SetDamage(damage);
    }
}
