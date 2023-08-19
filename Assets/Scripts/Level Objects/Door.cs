using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Door : MonoBehaviour
{
    public UnityEvent OnDoorOpen;

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
    [SerializeField] string openEndSoundName = "Door_Open_End";

    public bool Open => open;

    bool transitioning = false;
    List<Collider> agentsWithinRange = new List<Collider>();
    int openSoundID;
    int openEndSoundID;
    PositionalAudioSource doorOpenAudioSource;

    readonly float positionalTolerance = .01f;

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
        openSoundID = SoundManager.Instance.GetSoundID(openSoundName);
        if (openEndSoundName != "")
        {
            openEndSoundID = SoundManager.Instance.GetSoundID(openEndSoundName);
        }
        else
        {
            openEndSoundID = -1;
        }
    }

    private void Update()
    {
        if (transitioning)
        {
            if (open)
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, openPosition, doorMoveSpeed * Time.deltaTime);
                if (Vector3.Distance(doorTransform.localPosition, openPosition) < positionalTolerance)
                {
                    transitioning = false;
                    doorOpenAudioSource?.Stop();
                    if (openEndSoundID != -1)
                    {
                        SoundManager.Instance.PlaySoundAtPosition(openEndSoundID, transform.position);
                    }
                }
            }
            else
            {
                doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, closedPosition, doorMoveSpeed * Time.deltaTime);
                if (Vector3.Distance(doorTransform.localPosition, closedPosition) < positionalTolerance)
                {
                    transitioning = false;
                    doorOpenAudioSource?.Stop();
                    if (openEndSoundID != -1)
                    {
                        SoundManager.Instance.PlaySoundAtPosition(openEndSoundID, transform.position);
                    }
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
        if (open)
        {
            CloseDoor();
        }
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
        doorOpenAudioSource = SoundManager.Instance.PlaySoundAtPosition(openSoundID, transform.position);
        open = true;
        transitioning = true;
        OnDoorOpen.Invoke();
    }

    public void CloseDoor()
    {
        open = false;
        transitioning = true;
    }

    public void SetAutomatic(bool automatic)
    {
        this.automatic = automatic;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (automatic && !locked)
        {
            agentsWithinRange.Add(other);
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        agentsWithinRange.Remove(other);
        if (automatic && agentsWithinRange.Count == 0 && !locked)
        {
            CloseDoor();
        }
    }
}
