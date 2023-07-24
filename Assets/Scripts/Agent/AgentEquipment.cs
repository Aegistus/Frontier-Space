using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WeaponOffset
{
    Idle, Running, Aiming
}

public class AgentEquipment : MonoBehaviour
{
    [SerializeField] Transform weaponHoldTarget;

    public WeaponAttack CurrentWeaponAttack { get; private set; }
    GameObject CurrentWeapon { get; set; }
    public Holdable CurrentHoldable { get; set; }

    WeaponAnimation primaryWeapon;
    WeaponAnimation secondaryWeapon;

    HumanoidIK ik;
    HumanoidAnimator humanAnim;

    private void Start()
    {
        ik = GetComponentInChildren<HumanoidIK>();
        humanAnim = GetComponentInChildren<HumanoidAnimator>();
        primaryWeapon = GetComponentInChildren<WeaponAnimation>();
        Equip(primaryWeapon);
    }

    public void Equip(WeaponAnimation weapon)
    {
        CurrentHoldable = weapon.GetComponent<Holdable>();
        weapon.transform.SetParent(weaponHoldTarget);
        ik.SetHandTarget(Hand.Right, CurrentHoldable.RightHandPosition);
        ik.SetHandTarget(Hand.Left, CurrentHoldable.LeftHandPosition);
        humanAnim.SetAnimatorController(weapon.AnimationSet);
        CurrentWeaponAttack = weapon.GetComponent<WeaponAttack>();
        CurrentWeapon = weapon.gameObject;
    }

    public void SetWeaponOffset(WeaponOffset offsetType)
    {
        if (offsetType == WeaponOffset.Running)
        {
            CurrentWeapon.transform.localEulerAngles = CurrentHoldable.RunningRotation;
            CurrentWeapon.transform.localPosition = CurrentHoldable.RunningOffset;
        }
        else if (offsetType == WeaponOffset.Aiming)
        {
            CurrentWeapon.transform.localEulerAngles = CurrentHoldable.AimingRotation;
            CurrentWeapon.transform.localPosition = CurrentHoldable.AimingOffset;
        }
        else
        {
            CurrentWeapon.transform.localEulerAngles = CurrentHoldable.IdleRotation;
            CurrentWeapon.transform.localPosition = CurrentHoldable.IdleOffset;
        }
    }

}
