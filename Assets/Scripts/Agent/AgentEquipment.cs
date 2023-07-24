using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentEquipment : MonoBehaviour
{
    [SerializeField] Transform weaponHoldTarget;

    public WeaponAttack CurrentWeaponAttack { get; private set; }
    GameObject CurrentWeapon { get; set; }
    Holdable CurrentWeaponHoldable { get; set; }

    WeaponAnimation primaryWeapon;
    WeaponAnimation secondaryWeapon;

    HumanoidIK ik;
    HumanoidAnimator humanAnim;
    AgentMovement movement;
    AgentAction action;

    private void Start()
    {
        ik = GetComponentInChildren<HumanoidIK>();
        humanAnim = GetComponentInChildren<HumanoidAnimator>();
        primaryWeapon = GetComponentInChildren<WeaponAnimation>();
        movement = GetComponent<AgentMovement>();
        action = GetComponent<AgentAction>();
        movement.OnStateChange += ChangeWeaponOffset;
        action.OnStateChange += ChangeWeaponOffset;
        Equip(primaryWeapon);
    }

    public void Equip(WeaponAnimation weapon)
    {
        CurrentWeaponHoldable = weapon.GetComponent<Holdable>();
        weapon.transform.SetParent(weaponHoldTarget);
        ik.SetHandTarget(Hand.Right, CurrentWeaponHoldable.RightHandPosition);
        ik.SetHandTarget(Hand.Left, CurrentWeaponHoldable.LeftHandPosition);
        humanAnim.SetAnimatorController(weapon.AnimationSet);
        CurrentWeaponAttack = weapon.GetComponent<WeaponAttack>();
        CurrentWeapon = weapon.gameObject;
        ChangeWeaponOffset(movement.CurrentState);
    }

    void ChangeWeaponOffset(MovementState state)
    {
        if (state == MovementState.Run)
        {
            CurrentWeapon.transform.localEulerAngles = CurrentWeaponHoldable.RunningRotation;
            CurrentWeapon.transform.localPosition = CurrentWeaponHoldable.RunningOffset;
        }
        else
        {
            CurrentWeapon.transform.localEulerAngles = CurrentWeaponHoldable.IdleRotation;
            CurrentWeapon.transform.localPosition = CurrentWeaponHoldable.IdleOffset;
        }
    }

    void ChangeWeaponOffset(ActionState state)
    {
        if (state == ActionState.Aim)
        {
            CurrentWeapon.transform.localEulerAngles = CurrentWeaponHoldable.AimingRotation;
            CurrentWeapon.transform.localPosition = CurrentWeaponHoldable.AimingOffset;
        }
        else
        {
            CurrentWeapon.transform.localEulerAngles = CurrentWeaponHoldable.IdleRotation;
            CurrentWeapon.transform.localPosition = CurrentWeaponHoldable.IdleOffset;
        }
    }

}
