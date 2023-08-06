using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform doorTransform;
    [SerializeField] Vector3 closedPosition;
    [SerializeField] Vector3 openPosition;
    [SerializeField] float doorMoveSpeed = 1f;
    [SerializeField] bool open = false;

    public bool Open => open;

    bool transitioning = false;
    List<Collider> agentsWithinRange = new List<Collider>();

    private void Update()
    {
        if (transitioning)
        {
            if (open)
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, openPosition, doorMoveSpeed * Time.deltaTime);
                if (Vector3.Distance(doorTransform.localPosition, openPosition) < .01f)
                {
                    transitioning = false;
                }
            }
            else
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, closedPosition, doorMoveSpeed * Time.deltaTime);
                if (Vector3.Distance(doorTransform.localPosition, openPosition) < .01f)
                {
                    transitioning = false;
                }
            }
        }
    }

    public void OpenDoor()
    {
        open = true;
        transitioning = true;
    }

    public void CloseDoor()
    {
        open = false;
        transitioning = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        print("TEST");
        agentsWithinRange.Add(other);
        OpenDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        agentsWithinRange.Remove(other);
        if (agentsWithinRange.Count == 0)
        {
            CloseDoor();
        }
    }
}
