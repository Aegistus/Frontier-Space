using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer agentModel;

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
        agentModel.updateWhenOffscreen = true; // this prevents ragdolls from disappearing when the camera gets too close.
    }

    public void DisableRagdoll()
    {
        foreach (var rb in ragdollRBs)
        {
            rb.isKinematic = true;
        }
    }
}
