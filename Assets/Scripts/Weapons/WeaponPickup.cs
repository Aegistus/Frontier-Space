using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [SerializeField] string description;

    public string Description => description;

    public void Interact(GameObject interactor)
    {
        AgentEquipment equipment = interactor.GetComponent<AgentEquipment>();
        if (equipment.HasTwoWeapons)
        {
            equipment.DropWeapon();
        }
        equipment.PickupWeapon(gameObject);
    }
}
