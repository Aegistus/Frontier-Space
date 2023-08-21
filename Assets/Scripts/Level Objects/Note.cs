using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [SerializeField][TextArea] string contents;

    public string Description => "[E] Read Note";

    public void Interact(GameObject interactor)
    {
        NoteUI.DisplayNote(contents);
    }
}
