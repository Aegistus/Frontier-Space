using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    [SerializeField] float percentRefill = .5f;

    public string Description => "[E] Refill Ammo";

    public void Interact(GameObject interactor)
    {
        AgentEquipment equipment = interactor.GetComponent<AgentEquipment>();
        if (equipment)
        {
            equipment.RefillPercentAmmoAllWeapons(percentRefill);
        }
        gameObject.SetActive(false);
    }
}
