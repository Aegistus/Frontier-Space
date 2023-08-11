using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    public Transform destination;

    private void Awake()
    {
        GetComponent<NavMeshAgent>().SetDestination(destination.position);
    }
}
