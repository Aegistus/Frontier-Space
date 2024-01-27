using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollImpact : MonoBehaviour
{
    int groundLayer = 6;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            SoundManager.Instance.PlaySoundAtPosition("Ragdoll_Impact", collision.GetContact(0).point);
        }
    }
}
