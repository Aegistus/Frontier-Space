using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Transform doorTransform;
    [SerializeField] Light[] lights;
    [SerializeField] Color unlockedColor;
    [SerializeField] Color lockedColor;
    [SerializeField] Vector3 closedPosition;
    [SerializeField] Vector3 openPosition;
    [SerializeField] float doorMoveSpeed = 1f;
    [SerializeField] bool open = false;
    [SerializeField] bool automatic = true;
    [SerializeField] bool locked = false;
    [SerializeField] string openSoundName = "Door_Open";

    public bool Open => open;

    bool transitioning = false;
    List<Collider> agentsWithinRange = new List<Collider>();
    int soundID;

    private void Awake()
    {
        if (locked)
        {
            Lock();
        }
        else
        {
            Unlock();
        }
        if (open)
        {
            doorTransform.localPosition = openPosition;
        }
    }

    private void Start()
    {
        soundID = SoundManager.Instance.GetSoundID(openSoundName);
    }

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

    public void Lock()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].color = lockedColor;
        }
        locked = true;
    }

    public void Unlock()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].color = unlockedColor;
        }
        locked = false;
    }

    public void OpenDoor()
    {
        if (locked)
        {
            return;
        }
        SoundManager.Instance.PlaySoundAtPosition(soundID, transform.position);
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
        agentsWithinRange.Add(other);
        if (automatic)
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        agentsWithinRange.Remove(other);
        if (automatic && agentsWithinRange.Count == 0)
        {
            CloseDoor();
        }
    }
}
