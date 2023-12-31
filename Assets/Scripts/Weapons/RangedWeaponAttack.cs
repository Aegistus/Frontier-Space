using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeaponAttack : WeaponAttack
{
    [SerializeField] protected string projectileID;
    [SerializeField] protected string shootSoundName;
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected Transform weaponModel;
    [Space]
    [Header("Recoil")]
    [SerializeField] protected float recoilXRotation;
    [SerializeField] protected float recoilYRotation;
    [SerializeField] protected float recoilKickback = .1f;
    [SerializeField] protected float maxRecoilKickback = .1f;
    [SerializeField] protected float kickbackRecovery = .05f;
    [SerializeField] protected float recoilRotationRecovery = 1f;
    [SerializeField] protected float aimFOVChange = 10f;

    public Transform ProjectileSpawnPoint => projectileSpawnPoint;
    public float AimFOVChange => aimFOVChange;
    protected WeaponAmmunition weaponAmmo;
    protected int shootSoundID;

    float currentKickback;

    protected virtual void Awake()
    {
        weaponAmmo = GetComponent<WeaponAmmunition>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        shootSoundID = SoundManager.Instance.GetSoundID(shootSoundName);
    }

    protected virtual void Update()
    {
        weaponModel.localRotation = Quaternion.Slerp(weaponModel.localRotation, Quaternion.identity, recoilRotationRecovery * Time.deltaTime);
        weaponModel.localPosition = Vector3.Lerp(weaponModel.localPosition, Vector3.zero, kickbackRecovery * Time.deltaTime);
    }

    public void ApplyRecoil()
    {
        weaponModel.Rotate(Vector3.left, Random.Range(0, recoilXRotation));
        weaponModel.Rotate(Vector3.up, Random.Range(-recoilYRotation, recoilYRotation));
        float kick = Mathf.Clamp(currentKickback + recoilKickback, 0, maxRecoilKickback);
        transform.localPosition += Vector3.back * kick;
        OnRecoil.Invoke();
    }

    public virtual void SpawnProjectile()
    {
        if (weaponAmmo.TryUseAmmo())
        {
            GameObject projectile = PoolManager.Instance.SpawnObjectWithLifetime(projectileID, projectileSpawnPoint.position, projectileSpawnPoint.rotation, 10f);
            float damage = Random.Range(damageMin, damageMax);
            projectile.GetComponent<Projectile>().SetDamage(damage, Source);
            ApplyRecoil();
            SoundManager.Instance.PlaySoundAtPosition(shootSoundID, projectileSpawnPoint.position);
            PoolManager.Instance.SpawnObjectWithLifetime("Muzzle_Flash", projectileSpawnPoint.position, projectileSpawnPoint.rotation, 5f);
        }
    }

}
