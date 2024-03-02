using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IGCController : MonoBehaviour
{
    [System.Serializable]
    public class AnimationState
    {
        public string stateName;
        public float time;
        public UnityEvent before;
        public UnityEvent after;
    }

    public bool playWeaponAnimationsAtAwake = true;
    public Transform weapon;
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;
    public AnimationState[] animationStates;
    public AnimationState[] weaponAnimationStates;

    Animator anim;
    HumanoidIK ik;
    Animator weaponAnim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        ik = GetComponentInChildren<HumanoidIK>();
        if (weapon)
        {
            weaponAnim = weapon.GetComponent<Animator>();
        }
        if (leftHandIKTarget)
        {
            ik.SetHandTarget(Hand.Left, leftHandIKTarget, 100f);
            ik.SetHandWeight(Hand.Left, 1);
        }
        if (rightHandIKTarget)
        {
            ik.SetHandTarget(Hand.Right, rightHandIKTarget, 100f);
            ik.SetHandWeight(Hand.Right, 1);
        }
        StartCoroutine(PlayThroughAnimations());
        if (weaponAnim && playWeaponAnimationsAtAwake)
        {
            PlayWeaponAnimations();
        }
    }

    public void PlayWeaponAnimations()
    {
        StartCoroutine(PlayThroughWeaponAnimations());
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

    IEnumerator PlayThroughWeaponAnimations()
    {
        for (int i = 0; i < weaponAnimationStates.Length; i++)
        {
            if (weaponAnimationStates[i].stateName != "")
            {
                weaponAnim.CrossFadeInFixedTime(weaponAnimationStates[i].stateName, .5f);
            }
            weaponAnimationStates[i].before.Invoke();
            yield return new WaitForSeconds(weaponAnimationStates[i].time);
            weaponAnimationStates[i].after.Invoke();
        }
    }
}
