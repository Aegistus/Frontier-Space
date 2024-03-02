using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController animationSet;

    public RuntimeAnimatorController AnimationSet => animationSet;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Play(string name)
    {
        if (!anim.enabled)
        {
            anim.enabled = true;
        }
        anim.Play(name);
    }

    public void Stop()
    {
        anim.enabled = false;
    }
}
