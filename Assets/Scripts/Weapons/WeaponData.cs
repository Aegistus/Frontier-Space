using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    PlasmaRifle, SMG, Revolver, Shotgun, SniperRifle, TankRifle
}

public class WeaponData : MonoBehaviour
{
    [SerializeField] WeaponType type;

    public WeaponType Type => type;
}
