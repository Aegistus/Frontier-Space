using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePickup : MonoBehaviour, IInteractable
{
    [SerializeField] string description;
    [SerializeField] bool pickupable = true;

    public string Description => description;
    public bool CurrentlyInteractable { get; } = true;

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
        equipment.PickupGrenade(GetComponent<Grenade>());
    }
}
