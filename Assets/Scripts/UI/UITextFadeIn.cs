using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextFadeIn : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] float fadeSpeed = 1f;

    private void Awake()
    {
        Color color = text.color;
        color.a = 0;
        text.color = color;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        while (text.color.a < 1)
        {
            yield return null;
            Color color = text.color;
            color.a = Mathf.Lerp(color.a, 1, Time.deltaTime * fadeSpeed);
            text.color = color;
        }
    }
}
