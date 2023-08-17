using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAttack : RangedWeaponAttack
{
    [SerializeField] Vector3 spreadRotation;
    [SerializeField] Transform pump;
    [SerializeField] Vector3 pumpRestPosition;
    [SerializeField] Vector3 pumpedPosition;
    [SerializeField] float pumpStartDelay = .5f;
    [SerializeField] float pumpSpeed;

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
            StartCoroutine(Pump());
        }
    }

    IEnumerator Pump()
    {
        yield return new WaitForSeconds(pumpStartDelay);
        SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetSoundID("Shotgun_Pump"), transform.position);
        float pumpTimer = 0;
        while (pumpTimer < pumpDelay / 2)
        {
            pump.localPosition = Vector3.Lerp(pump.localPosition, pumpedPosition, Time.deltaTime * pumpSpeed);
            yield return null;
            pumpTimer += Time.deltaTime;
        }
        while (pumpTimer < pumpDelay)
        {
            pump.localPosition = Vector3.Lerp(pump.localPosition, pumpRestPosition, Time.deltaTime * pumpSpeed);
            yield return null;
            pumpTimer += Time.deltaTime;
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
