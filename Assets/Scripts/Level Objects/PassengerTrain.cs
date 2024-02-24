using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerTrain : MonoBehaviour
{
    [SerializeField] bool moving = false;
    [SerializeField] float moveSpeed;
    [SerializeField] float moveSpeedCap = 10f;
    [SerializeField] Vector3 destination;
    [SerializeField] Door[] doors;

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
            Vector3 velocity = Vector3.Lerp(transform.position, destination, moveSpeed * Time.deltaTime) - transform.position;
            velocity = Vector3.ClampMagnitude(velocity, moveSpeedCap * Time.deltaTime);
            transform.position += velocity;
        }
        if (moving && Vector3.Distance(transform.position, destination) <= arrivalDistance)
        {
            moving = false;
            OpenDoors();
        }
    }
}
