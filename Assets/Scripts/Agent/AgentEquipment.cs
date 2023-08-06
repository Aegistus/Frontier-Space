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
    [SerializeField] float weaponOffsetChangeSpeed = 1f;

    public WeaponAttack CurrentWeaponAttack { get; private set; }
    GameObject CurrentWeapon { get; set; }
    public Holdable CurrentHoldable { get; set; }

    WeaponAnimation primaryWeapon;
    WeaponAnimation secondaryWeapon;

    HumanoidIK ik;
    HumanoidAnimator humanAnim;

    Vector3 targetPosition;
    Vector3 targetRotation;

    private void Start()
    {
        ik = GetComponentInChildren<HumanoidIK>();
        humanAnim = GetComponentInChildren<HumanoidAnimator>();
        primaryWeapon = GetComponentInChildren<WeaponAnimation>();
        Equip(primaryWeapon);
        SetWeaponOffset(WeaponOffset.Idle);
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
            targetRotation = CurrentHoldable.RunningRotation;
            targetPosition = CurrentHoldable.RunningOffset;
        }
        else if (offsetType == WeaponOffset.Aiming)
        {
            targetRotation = CurrentHoldable.AimingRotation;
            targetPosition = CurrentHoldable.AimingOffset;
        }
        else
        {
            targetRotation = CurrentHoldable.IdleRotation;
            targetPosition = CurrentHoldable.IdleOffset;
        }
    }

    private void Update()
    {
        if (CurrentWeapon)
        {
            CurrentWeapon.transform.localEulerAngles = Vector3.SlerpUnclamped(CurrentWeapon.transform.localEulerAngles, targetRotation, weaponOffsetChangeSpeed * Time.deltaTime);
            CurrentWeapon.transform.localPosition = Vector3.Lerp(CurrentWeapon.transform.localPosition, targetPosition, weaponOffsetChangeSpeed * Time.deltaTime);
        }
    }

}
