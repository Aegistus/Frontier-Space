using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCameraManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraShot
    {
        public GameObject camera;
        public float time = 5f;
    }

    public CameraShot[] cameraShots;

    private void Start()
    {
        for (int i = 0; i < cameraShots.Length; i++)
        {
            cameraShots[i].camera.SetActive(false);
        }
        StartCoroutine(AdvanceThroughCameras());
    }

    IEnumerator AdvanceThroughCameras()
    {
        for (int i = 0; i < cameraShots.Length; i++)
        {
            cameraShots[i].camera.SetActive(true);
            yield return new WaitForSeconds(cameraShots[i].time);
            cameraShots[i].camera.SetActive(false);
        }
    }
}
