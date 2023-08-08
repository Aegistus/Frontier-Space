using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryTeller : MonoBehaviour
{
    public StoryEvent[] storyEvents;

    [System.Serializable]
    public class StoryEvent
    {
        public string name;
        public UnityEvent OnEventTrigger;
    }
}
