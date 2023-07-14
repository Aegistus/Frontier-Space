using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentEquipment : MonoBehaviour
{
    [SerializeField] Transform weaponHoldTarget;

    Weapon primaryWeapon;
    Weapon secondaryWeapon;

    HumanoidIK ik;
    HumanoidAnimator humanAnim;

    private void Start()
    {
        ik = GetComponentInChildren<HumanoidIK>();
        humanAnim = GetComponentInChildren<HumanoidAnimator>();
        primaryWeapon = GetComponentInChildren<Weapon>();
        Equip(primaryWeapon);
    }

    public void Equip(Weapon weapon)
    {
        Holdable holdable = weapon.GetComponent<Holdable>();
        weapon.transform.SetParent(weaponHoldTarget);
        weapon.transform.localPosition = holdable.Offset;
        ik.SetHandTarget(Hand.Right, holdable.RightHandPosition);
        ik.SetHandTarget(Hand.Left, holdable.LeftHandPosition);
        humanAnim.SetAnimatorController(weapon.AnimationSet);
    }


}
