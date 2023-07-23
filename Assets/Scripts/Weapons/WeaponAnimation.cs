using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController animationSet;

    public RuntimeAnimatorController AnimationSet => animationSet;
}
