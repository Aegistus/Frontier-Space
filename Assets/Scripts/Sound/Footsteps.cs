using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    SoundManager soundManager;
    int lightFootstep;
    int heavyFootstep;

    private void Start()
    {
        soundManager = SoundManager.Instance;
        lightFootstep = soundManager.GetSoundID("Footstep_Walk");
        heavyFootstep = soundManager.GetSoundID("Footstep_Run");
    }

    private void Step()
    {
        soundManager.PlaySoundAtPosition(lightFootstep, transform.position);
    }

    private void HeavyStep()
    {
        soundManager.PlaySoundAtPosition(heavyFootstep, transform.position);
    }
}