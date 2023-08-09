using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] Vector3 rotationAngles;

    private void Update()
    {
        transform.Rotate(rotationAngles * Time.deltaTime, Space.Self);
    }
}
