using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, IInteractable
{
    public UnityEvent OnPress;

    public void Interact()
    {
        OnPress.Invoke();
    }
}
