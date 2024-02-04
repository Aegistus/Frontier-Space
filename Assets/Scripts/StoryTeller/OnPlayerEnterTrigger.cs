using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnPlayerEnterTrigger : MonoBehaviour
{
    public UnityEvent OnTrigger;

    bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyTriggered)
        {
            return;
        }
        if (other.GetComponent<PlayerController>())
        {
            OnTrigger.Invoke();
            alreadyTriggered = true;
        }
    }
}
