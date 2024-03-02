using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSounds : MonoBehaviour
{
    public void MagazineOut()
    {
        SoundManager.Instance.PlaySoundAtPosition("Magazine_Out", transform.position);
    }

    public void MagazineIn()
    {
        SoundManager.Instance.PlaySoundAtPosition("Magazine_In", transform.position);
    }

    public void SlidePull()
    {
        SoundManager.Instance.PlaySoundAtPosition("Slide_Pull", transform.position);
    }

    public void SlideRelease()
    {
        SoundManager.Instance.PlaySoundAtPosition("Slide_Release", transform.position);
    }

    public void CockHammer()
    {
        SoundManager.Instance.PlaySoundAtPosition("Revolver_Cock", transform.position);
    }

    public void PumpShotgun()
    {
        SoundManager.Instance.PlaySoundAtPosition("Shotgun_Pump", transform.position);
    }
}
