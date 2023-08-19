using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    public string Description { get; }
    void Interact(GameObject interactor);
}
