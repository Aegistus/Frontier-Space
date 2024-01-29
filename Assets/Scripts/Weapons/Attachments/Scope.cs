using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    [SerializeField] Camera scopeCamera;
    [SerializeField] GameObject canvas;

    WeaponAttack thisWeapon;
    AgentEquipment playerEquipment;

    private void Awake()
    {
        scopeCamera.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        thisWeapon = GetComponentInParent<WeaponAttack>();
        playerEquipment = FindObjectOfType<PlayerController>().GetComponent<AgentEquipment>();
        playerEquipment.OnWeaponChange += OnWeaponChange;
    }

    private void OnWeaponChange()
    {
        if (playerEquipment.CurrentWeaponAttack == thisWeapon)
        {
            scopeCamera.gameObject.SetActive(true);
            canvas.gameObject.SetActive(true);
        }
        else
        {
            scopeCamera.gameObject.SetActive(false);
            canvas.gameObject.SetActive(false);
        }
    }
}
