using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArmorTube : MonoBehaviour, IInteractable
{
    public string Description => "[E] Equip Armor";
    public UnityEvent OnEquip;

    public void Interact()
    {
        OnEquip.Invoke();
    }
}
