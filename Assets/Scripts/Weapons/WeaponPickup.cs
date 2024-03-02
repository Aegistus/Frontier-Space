using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [SerializeField] string description;
    [SerializeField] bool pickupable = true;

    public string Description => description;
    public bool CurrentlyInteractable
    { 
        get { return pickupable; }
        set { pickupable = value; }
    }

    public void Interact(GameObject interactor)
    {
        AgentEquipment equipment = interactor.GetComponent<AgentEquipment>();
        equipment.PickupWeapon(gameObject);
    }
}
