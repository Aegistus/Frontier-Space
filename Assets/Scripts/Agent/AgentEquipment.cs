using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum WeaponOffset
{
    Idle, Running, Aiming, Reloading, Crouching
}

public class AgentEquipment : MonoBehaviour
{
    [SerializeField] Transform weaponHoldTarget;
    [SerializeField] float weaponOffsetChangeSpeed = 1f;

    public event Action OnWeaponChange;

    public bool HasWeaponEquipped => CurrentWeapon != null;
    public WeaponAttack CurrentWeaponAttack { get; private set; }
    public WeaponAmmunition CurrentWeaponAmmunition { get; private set; }
    GameObject CurrentWeapon { get; set; }
    public Holdable CurrentHoldable { get; set; }

    WeaponAnimation primaryWeapon;
    WeaponAnimation secondaryWeapon;

    HumanoidIK ik;
    HumanoidAnimator humanAnim;

    Vector3 targetPosition;
    Quaternion targetRotation;

    private void Start()
    {
        ik = GetComponentInChildren<HumanoidIK>();
        humanAnim = GetComponentInChildren<HumanoidAnimator>();
        primaryWeapon = GetComponentInChildren<WeaponAnimation>();
        if (primaryWeapon)
        {
            Equip(primaryWeapon);
            SetWeaponOffset(WeaponOffset.Idle);
        }
    }

    public void Equip(WeaponAnimation weapon)
    {
        CurrentHoldable = weapon.GetComponent<Holdable>();
        weapon.transform.SetParent(weaponHoldTarget);
        ik.SetHandTarget(Hand.Right, CurrentHoldable.RightHandPosition);
        ik.SetHandTarget(Hand.Left, CurrentHoldable.LeftHandPosition);
        humanAnim.SetAnimatorController(weapon.AnimationSet);
        CurrentWeaponAttack = weapon.GetComponent<WeaponAttack>();
        CurrentWeaponAmmunition = weapon.GetComponent<WeaponAmmunition>();
        CurrentWeapon = weapon.gameObject;
        OnWeaponChange?.Invoke();
    }

    public void SetWeaponOffset(WeaponOffset offsetType)
    {
        if (!HasWeaponEquipped)
        {
            return;
        }
        if (offsetType == WeaponOffset.Running)
        {
            targetRotation = Quaternion.Euler(CurrentHoldable.RunningRotation);
            targetPosition = CurrentHoldable.RunningOffset;
        }
        else if (offsetType == WeaponOffset.Aiming)
        {
            targetRotation = Quaternion.Euler(CurrentHoldable.AimingRotation);
            targetPosition = CurrentHoldable.AimingOffset;
        }
        else if (offsetType == WeaponOffset.Reloading)
        {
            targetRotation = Quaternion.Euler(CurrentHoldable.ReloadRotation);
            targetPosition = CurrentHoldable.ReloadOffset;
        }
        else if (offsetType == WeaponOffset.Crouching)
        {
            targetRotation = Quaternion.Euler(CurrentHoldable.CrouchingRotation);
            targetPosition = CurrentHoldable.CrouchingOffset;
        }
        else
        {
            targetRotation = Quaternion.Euler(CurrentHoldable.IdleRotation);
            targetPosition = CurrentHoldable.IdleOffset;
        }
    }

    private void Update()
    {
        if (CurrentWeapon)
        {
            CurrentWeapon.transform.localRotation = Quaternion.Lerp(CurrentWeapon.transform.localRotation, targetRotation, weaponOffsetChangeSpeed * Time.deltaTime);
            CurrentWeapon.transform.localPosition = Vector3.Lerp(CurrentWeapon.transform.localPosition, targetPosition, weaponOffsetChangeSpeed * Time.deltaTime);
        }
    }

    public void DropWeapon()
    {
        CurrentWeapon.transform.SetParent(null, true);
        Rigidbody weaponRB = CurrentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = false;
        CurrentWeapon = null;
        CurrentHoldable = null;
        CurrentWeaponAttack = null;
        CurrentWeaponAmmunition = null;
    }

}
