using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    [SerializeField] string description;
    [SerializeField] bool pickupable = true;

    public string Description => description;

    void Start()
    {
        if (!pickupable)
        {
            Destroy(this);
        }
    }

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
