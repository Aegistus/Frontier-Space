using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public event Action<string> OnDialogueStart; // passes dialogue caption string
    public event Action OnDialogueEnd;

    [System.Serializable]
    public class Dialogue
    {
        public string id;
        public string caption;
    }

    public Dialogue[] dialogueLines;

    bool isPlaying = false;

    Queue<Dialogue> toPlay = new Queue<Dialogue>();
    AudioSource currentAudioSource;

    public void QueueDialogueLine(string id)
    {
        Dialogue dialogue = Array.Find(dialogueLines, d => d.id == id);
        if (currentAudioSource == null || !currentAudioSource.isPlaying)
        {
            PlayDialogue(dialogue);
        }
        else
        {
            toPlay.Enqueue(dialogue);
        }
    }

    private void Update()
    {
        if (currentAudioSource != null && !currentAudioSource.isPlaying && Time.timeScale != 0)
        {
            if (toPlay.Count > 0)
            {
                PlayDialogue(toPlay.Dequeue());
            }
            else if (isPlaying == true)
            {
                isPlaying = false;
                OnDialogueEnd?.Invoke();
            }
        }
    }

    void PlayDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            return;
        }
        int soundID = SoundManager.Instance.GetSoundID(dialogue.id);
        currentAudioSource = SoundManager.Instance.PlaySoundGlobal(soundID);
        isPlaying = true;
        OnDialogueStart?.Invoke(dialogue.caption);
    }

    private void OnDestroy()
    {
        toPlay.Clear();
    }
}
