using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAttack : RangedWeaponAttack
{
    [SerializeField] Vector3 spreadRotation;
    int shootSoundID;
    int pelletCount = 10;
    float pumpDelay = .75f;
    float timer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        shootSoundID = SoundManager.Instance.GetSoundID("Shotgun_Shoot");
    }

    public override void BeginAttack()
    {
        if (timer <= 0)
        {
            SpawnProjectile();
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
            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 eulerAngles = projectileSpawnPoint.eulerAngles;
                eulerAngles += new Vector3(Random.Range(-spreadRotation.x, spreadRotation.x), Random.Range(-spreadRotation.y, spreadRotation.y), Random.Range(-spreadRotation.z, spreadRotation.z));
                Quaternion rotation = Quaternion.Euler(eulerAngles);
                GameObject projectile = PoolManager.Instance.SpawnObjectWithLifetime(projectileID, projectileSpawnPoint.position, rotation, 10f);
                float damage = Random.Range(damageMin, damageMax);
                projectile.GetComponent<Projectile>().SetDamage(damage);
            }
            ApplyRecoil();
            SoundManager.Instance.PlaySoundAtPosition(shootSoundID, projectileSpawnPoint.position);
            timer = pumpDelay;
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
