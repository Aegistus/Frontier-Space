using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    [SerializeField] protected float damageMin = 10f;
    [SerializeField] protected float damageMax = 20f;

    public abstract void BeginAttack();
    public abstract void DuringAttack();
    public abstract void EndAttack();
}
