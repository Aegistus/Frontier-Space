using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoTrain : MonoBehaviour
{
    [SerializeField] Vector3 startingPoint;
    [SerializeField] Vector3 endPoint;

    readonly float resetDistance = 10f;

    private void Update()
    {
        if (Vector3.Distance(transform.position, endPoint) <= resetDistance)
        {
            transform.position = startingPoint;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        AgentHealth health = other.GetComponentInParent<AgentHealth>();
        if (health != null)
        {
            health.Kill();
        }
    }
}
