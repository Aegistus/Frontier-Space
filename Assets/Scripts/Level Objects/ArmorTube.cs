using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArmorTube : MonoBehaviour, IInteractable
{
    public string Description => "[E] Equip Armor";
    public bool CurrentlyInteractable { get; set; } = true;
    public UnityEvent OnEquip;


    public void Interact(GameObject _)
    {
        if (!CurrentlyInteractable)
        {
            return;
        }
        OnEquip.Invoke();
        CurrentlyInteractable = false;
    }
}
