using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] ragdollRBs;
    Animator anim;

    private void Awake()
    {
        ragdollRBs = GetComponentsInChildren<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    public void EnableRagdoll()
    {
        anim.enabled = false;
        foreach (var rb in ragdollRBs)
        {
            rb.isKinematic = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach (var rb in ragdollRBs)
        {
            rb.isKinematic = true;
        }
    }
}
