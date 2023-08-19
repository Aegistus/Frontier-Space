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

    public bool HasWeaponEquipped => CurrentWeaponGO != null;
    public bool HasTwoWeapons => primaryWeapon != null && secondaryWeapon != null;
    public WeaponAttack CurrentWeaponAttack => currentWeapon?.attack;
    public WeaponAmmunition CurrentWeaponAmmunition => currentWeapon?.ammo;
    GameObject CurrentWeaponGO => currentWeapon?.gameObject;
    public Holdable CurrentHoldable => currentWeapon?.holdable;

    Weapon currentWeapon;

    Weapon primaryWeapon;
    Weapon secondaryWeapon;

    HumanoidIK ik;
    HumanoidAnimator humanAnim;

    Vector3 targetPosition;
    Quaternion targetRotation;

    public class Weapon
    {
        public GameObject gameObject;
        public WeaponAnimation animation;
        public WeaponAttack attack;
        public WeaponAmmunition ammo;
        public Holdable holdable;

        public Weapon(GameObject gameObject)
        {
            this.gameObject = gameObject;
            attack = gameObject.GetComponent<WeaponAttack>();
            animation = gameObject.GetComponent<WeaponAnimation>();
            ammo = gameObject.GetComponent<WeaponAmmunition>();
            holdable = gameObject.GetComponent<Holdable>();
        }
    }

    private void Start()
    {
        ik = GetComponentInChildren<HumanoidIK>();
        humanAnim = GetComponentInChildren<HumanoidAnimator>();
        WeaponAttack[] weaponAttacks = GetComponentsInChildren<WeaponAttack>();
        if (weaponAttacks.Length > 0 && weaponAttacks[0] != null)
        {
            primaryWeapon = new Weapon(weaponAttacks[0].gameObject);
        }
        if (weaponAttacks.Length > 1 && weaponAttacks[1] != null)
        {
            secondaryWeapon = new Weapon(weaponAttacks[1].gameObject);
        }
        if (primaryWeapon != null)
        {
            Equip(primaryWeapon);
        }
        if (secondaryWeapon != null)
        {
            UnEquip(secondaryWeapon);
        }
    }

    public void Equip(Weapon weapon)
    {
        currentWeapon = weapon;
        currentWeapon.gameObject.SetActive(true);
        weapon.gameObject.transform.SetParent(weaponHoldTarget);
        ik.SetHandTarget(Hand.Right, CurrentHoldable.RightHandPosition);
        ik.SetHandTarget(Hand.Left, CurrentHoldable.LeftHandPosition);
        humanAnim.SetAnimatorController(weapon.animation.AnimationSet);
        SetWeaponOffset(WeaponOffset.Idle);
        OnWeaponChange?.Invoke();
    }

    public void UnEquip(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
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
        if (CurrentWeaponGO)
        {
            CurrentWeaponGO.transform.localRotation = Quaternion.Lerp(CurrentWeaponGO.transform.localRotation, targetRotation, weaponOffsetChangeSpeed * Time.deltaTime);
            CurrentWeaponGO.transform.localPosition = Vector3.Lerp(CurrentWeaponGO.transform.localPosition, targetPosition, weaponOffsetChangeSpeed * Time.deltaTime);
        }
    }

    public void PickupWeapon(GameObject weaponGO)
    {
        weaponGO.transform.SetParent(weaponHoldTarget);
        Rigidbody weaponRB = weaponGO.GetComponent<Rigidbody>();
        weaponRB.isKinematic = true;
        Weapon newWeapon = new Weapon(weaponGO);
        weaponGO.GetComponent<BoxCollider>().enabled = false;
        if (primaryWeapon == null)
        {
            primaryWeapon = newWeapon;
        }
        else if (secondaryWeapon == null)
        {
            secondaryWeapon = newWeapon;
        }
        if (currentWeapon != null)
        {
            UnEquip(currentWeapon);
        }
        Equip(newWeapon);
    }

    public void DropWeapon()
    {
        if (currentWeapon != null)
        {
            if (currentWeapon == primaryWeapon)
            {
                primaryWeapon = null;
            }
            else
            {
                secondaryWeapon = null;
            }
            CurrentWeaponGO.transform.SetParent(null, true);
            Rigidbody weaponRB = CurrentWeaponGO.GetComponent<Rigidbody>();
            weaponRB.isKinematic = false;
            CurrentWeaponGO.GetComponent<BoxCollider>().enabled = true;
            currentWeapon = null;
        }
    }

    public bool TrySwitchWeapon()
    {
        if (secondaryWeapon == null)
        {
            return false;
        }
        if (currentWeapon == primaryWeapon)
        {
            UnEquip(primaryWeapon);
            Equip(secondaryWeapon);
        }
        else
        {
            UnEquip(secondaryWeapon);
            Equip(primaryWeapon);
        }
        return true;
    }

}
