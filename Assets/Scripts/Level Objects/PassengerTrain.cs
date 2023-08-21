using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerTrain : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 destination;
    [SerializeField] Door[] doors;

    bool moving = false;

    readonly float arrivalDistance = 1;

    public void CallTrain()
    {
        moving = true;
    }

    public void OpenDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].OpenDoor();
        }
    }

    public void CloseDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].CloseDoor();
        }
    }

    private void Update()
    {
        if (moving)
        {
            transform.position = Vector3.Lerp(transform.position, destination, moveSpeed * Time.deltaTime);
        }
        if (moving && Vector3.Distance(transform.position, destination) <= arrivalDistance)
        {
            moving = false;
            OpenDoors();
        }
    }
}
