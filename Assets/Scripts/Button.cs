using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, IInteractable
{
    public UnityEvent OnPress;

    int soundID;

    void Start()
    {
        soundID = SoundManager.Instance.GetSoundID("Button_Press");
    }

    public void Interact()
    {
        OnPress.Invoke();
        SoundManager.Instance.PlaySoundGlobal(soundID);
    }
}
