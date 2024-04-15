using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update()
    {
        transform.LookAt(target);
    }
}
