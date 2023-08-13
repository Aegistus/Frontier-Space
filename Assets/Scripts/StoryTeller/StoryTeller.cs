using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class StoryTeller : MonoBehaviour
{
    [SerializeField] string playEventOnAwake = "";
    public StoryEvent[] storyEvents;

    [Serializable]
    public class StoryEvent
    {
        public string name;
        public float delay;
        public UnityEvent OnEventTrigger;
    }

    private void Awake()
    {
        if (playEventOnAwake != "")
        {
            TriggerEvent(playEventOnAwake);
        }
    }

    public void TriggerEvent(string name)
    {
        StoryEvent storyEvent = Array.Find(storyEvents, e => e.name == name);
        if (storyEvent == null)
        {
            Debug.LogError("Story Event: " + name + " does not exist.");
        }
        StartCoroutine(EventCoroutine(storyEvent));
    }

    IEnumerator EventCoroutine(StoryEvent storyEvent)
    {
        yield return new WaitForSeconds(storyEvent.delay);
        storyEvent.OnEventTrigger.Invoke();
    }
}
