using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeaponAttack : WeaponAttack
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform projectileSpawnPoint;
}
