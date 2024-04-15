using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] Vector3 movement;

    private void Update()
    {
        transform.Translate(movement * Time.deltaTime, Space.Self);
    }
}
