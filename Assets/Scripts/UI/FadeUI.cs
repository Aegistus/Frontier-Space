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
        StartCoroutine(FadeCorutine(0));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCorutine(1));
    }

    IEnumerator FadeCorutine(float targetAlpha)
    {
        element.gameObject.SetActive(true);
        while (element.color.a != targetAlpha)
        {
            yield return null;
            Color color = element.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
            if (Mathf.Abs(color.a - targetAlpha) < approximation)
            {
                color.a = targetAlpha;
            }
            element.color = color;
        }
    }
}
