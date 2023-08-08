using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class StoryTeller : MonoBehaviour
{
    public StoryEvent[] storyEvents;

    [System.Serializable]
    public class StoryEvent
    {
        public string name;
        public UnityEvent OnEventTrigger;
    }

    public void TriggerEvent(string name)
    {
        StoryEvent storyEvent = Array.Find(storyEvents, e => e.name == name);
        storyEvent.OnEventTrigger.Invoke();
    }
}
