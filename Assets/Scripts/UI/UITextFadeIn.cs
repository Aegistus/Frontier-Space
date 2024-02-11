using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextFadeIn : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] float fadeInDelay = 0f;
    [SerializeField] float fadeSpeed = 1f;
    [SerializeField] bool fadeOut = false;
    [SerializeField] float fadeOutDelay = 5f;

    readonly float alphaLeeway = .01f;

    private void Awake()
    {
        Color color = text.color;
        color.a = 0;
        text.color = color;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        yield return new WaitForSeconds(fadeInDelay);
        while (text.color.a < 1)
        {
            yield return null;
            Color color = text.color;
            color.a = Mathf.Lerp(color.a, 1, Time.deltaTime * fadeSpeed);
            if (Mathf.Abs(1 - color.a ) < alphaLeeway)
            {
                color.a = 1;
            }
            text.color = color;
        }
        if (fadeOut)
        {
            StartCoroutine(FadeDelayCoroutine());
        }
    }

    IEnumerator FadeDelayCoroutine()
    {
        yield return new WaitForSeconds(fadeOutDelay);
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        while (text.color.a > 0)
        {
            yield return null;
            Color color = text.color;
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * fadeSpeed);
            if (Mathf.Abs(color.a) < alphaLeeway)
            {
                color.a = 0;
            }
            text.color = color;
        }
    }
}
