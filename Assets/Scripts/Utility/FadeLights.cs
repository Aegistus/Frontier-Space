using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeLights : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 5f;

    Light targetLight;
    float maxIntensity;

    private void Awake()
    {
        targetLight = GetComponentInChildren<Light>();
        maxIntensity = targetLight.intensity;
    }

    public void FadeInAndOut()
    {
        StartCoroutine(FadeInAndOutCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeInAndOutCoroutine()
    {
        while (true)
        {
            while (targetLight.intensity > 0)
            {
                yield return null;
                targetLight.intensity -= fadeSpeed * Time.deltaTime;
            }
            while (targetLight.intensity < maxIntensity)
            {
                yield return null;
                targetLight.intensity += fadeSpeed * Time.deltaTime;
            }
        }
    }

    IEnumerator FadeOutCoroutine()
    {
        while (targetLight.intensity > 0)
        {
            yield return null;
            targetLight.intensity -= fadeSpeed * Time.deltaTime;
        }
    }

    IEnumerator FadeInCoroutine()
    {
        while (targetLight.intensity < maxIntensity)
        {
            yield return null;
            targetLight.intensity += fadeSpeed * Time.deltaTime;
        }
    }
}
