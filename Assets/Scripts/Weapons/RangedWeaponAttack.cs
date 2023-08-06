using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeaponAttack : WeaponAttack
{
    [SerializeField] protected string projectileID;
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected Transform weaponModel;
    [SerializeField] protected float recoilXRotation;
    [SerializeField] protected float recoilYRotation;
    [SerializeField] protected float recoilRotationRecovery = 1f;

    public Transform ProjectileSpawnPoint => projectileSpawnPoint;

    public void ApplyRecoil()
    {
        weaponModel.Rotate(Vector3.left, recoilXRotation);
        weaponModel.Rotate(Vector3.up, Random.Range(-recoilYRotation, recoilYRotation));
    }

    protected virtual void Update()
    {
        weaponModel.localRotation = Quaternion.Slerp(weaponModel.localRotation, Quaternion.identity, recoilRotationRecovery * Time.deltaTime);
    }

}
