using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CinematicController : MonoBehaviour
{
    [System.Serializable]
    public class AnimationState
    {
        public string stateName;
        public float time;
        public UnityEvent before;
        public UnityEvent after;
    }

    public AnimationState[] animationStates;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(PlayThroughAnimations());
    }

    IEnumerator PlayThroughAnimations()
    {
        for (int i = 0; i < animationStates.Length; i++)
        {
            if (animationStates[i].stateName != "")
            {
                anim.CrossFadeInFixedTime(animationStates[i].stateName, .5f);
            }
            animationStates[i].before.Invoke();
            yield return new WaitForSeconds(animationStates[i].time);
            animationStates[i].after.Invoke();
        }
    }
}
