using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform target;

    private void LateUpdate()
    {
        transform.rotation = target.rotation;
        transform.position = target.position;
    }
}
