using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] string description;

    public UnityEvent OnPress;
    public string Description => description;

    int soundID;

    void Start()
    {
        soundID = SoundManager.Instance.GetSoundID("Button_Press");
    }

    public void Interact(GameObject _)
    {
        OnPress.Invoke();
        SoundManager.Instance.PlaySoundGlobal(soundID);
    }
}
