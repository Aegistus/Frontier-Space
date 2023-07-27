using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WeaponHoldTarget : MonoBehaviour
{
    [SerializeField] Transform reference;
    [SerializeField] float moveSpeed = .1f;


    private void LateUpdate()
    {
        if (reference)
        {
            transform.position = reference.position;
            transform.rotation = reference.rotation;
        }
    }
}
