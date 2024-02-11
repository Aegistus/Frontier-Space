using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CinematicCameraManager : MonoBehaviour
{
    public UnityEvent<Camera> OnCameraSwitch;

    [System.Serializable]
    public class CameraShot
    {
        public GameObject camera;
        public float time = 5f;
        public UnityEvent trigger;
        public UnityEvent after;
    }

    public CameraShot[] cameraShots;

    private void Start()
    {
        for (int i = 0; i < cameraShots.Length; i++)
        {
            if (cameraShots[i].camera != null)
            {
                cameraShots[i].camera.SetActive(false);
            }
        }
        StartCoroutine(AdvanceThroughCameras());
    }

    IEnumerator AdvanceThroughCameras()
    {
        for (int i = 0; i < cameraShots.Length; i++)
        {
            if (cameraShots[i].camera != null)
            {
                cameraShots[i].camera.SetActive(true);
                OnCameraSwitch?.Invoke(cameraShots[i].camera.GetComponent<Camera>());
            }
            cameraShots[i].trigger.Invoke();
            yield return new WaitForSeconds(cameraShots[i].time);
            cameraShots[i].after.Invoke();
            if (cameraShots[i].camera != null)
            {
                cameraShots[i].camera.SetActive(false);
            }
        }
    }
}
