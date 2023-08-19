using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWarningLights : MonoBehaviour
{
    [SerializeField] Light[] lights;
    [SerializeField] float flashInterval = 2f;
    [SerializeField] float flashLength = 10f;

    float startingIntensity;

    public void StartFlashing()
    {
        startingIntensity = lights[0].intensity;
        StartCoroutine(FlashingCoroutine());
    }

    IEnumerator FlashingCoroutine()
    {
        float timer = 0f;
        while (timer < flashLength)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].gameObject.SetActive(true);
            }
        }
    }
}
