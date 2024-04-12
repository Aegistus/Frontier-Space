using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeChildOf : MonoBehaviour
{
    [SerializeField] Transform parent;

    private void Start()
    {
        transform.SetParent(parent);
    }
}
