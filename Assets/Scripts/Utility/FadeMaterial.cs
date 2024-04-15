using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterial : MonoBehaviour
{
    [SerializeField] Renderer rend;
    [Range(0f,1f)][SerializeField] float startValue = .9f;
    [Range(0f, 1f)] [SerializeField] float endValue = 0f;
    [SerializeField] float time = 5f;

    float speed;

    private void Start()
    {
        speed = 1f / time;
        Color color = rend.material.color;
        color.a = startValue;
        rend.material.color = color;
    }

    public void BeginFade()
    {
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        Color color;
        do
        {
            color = rend.material.color;
            if (startValue > endValue)
            {
                color.a -= Time.deltaTime * speed;
            }
            else
            {
                color.a += Time.deltaTime * speed;
            }
            rend.material.color = color;
            yield return null;
        }
        while (color.a != endValue);
    }

}
