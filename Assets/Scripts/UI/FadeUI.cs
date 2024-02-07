using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 1f;
    [SerializeField] Image element;
    [SerializeField] bool fadeInOnAwake = false;

    readonly float approximation = .01f;

    private void Awake()
    {
        if (fadeInOnAwake)
        {
            FadeIn();
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCorutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeInCorutine()
    {
        element.gameObject.SetActive(true);
        if (element.color.a == 0)
        {
            Color c = element.color;
            c.a = 1;
            element.color = c;
        }
        while (element.color.a != 0)
        {
            yield return null;
            Color color = element.color;
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * fadeSpeed);
            if (Mathf.Abs(color.a - 0) < approximation)
            {
                color.a = 0;
            }
            element.color = color;
        }
    }

    IEnumerator FadeOutCoroutine()
    {
        element.gameObject.SetActive(true);
        if (element.color.a == 1)
        {
            Color c = element.color;
            c.a = 0;
            element.color = c;
        }
        while (element.color.a != 1)
        {
            yield return null;
            Color color = element.color;
            color.a = Mathf.Lerp(color.a, 1, Time.deltaTime * fadeSpeed);
            if (Mathf.Abs(color.a - 1) < approximation)
            {
                color.a = 1;
            }
            element.color = color;
        }
    }
}
