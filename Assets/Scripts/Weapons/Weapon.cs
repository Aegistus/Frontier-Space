using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController animationSet;

    public RuntimeAnimatorController AnimationSet => animationSet;
}
