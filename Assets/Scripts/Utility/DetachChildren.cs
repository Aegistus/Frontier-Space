using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachChildren : MonoBehaviour
{
    private void Awake()
    {
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
