using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorUI : MonoBehaviour
{
    [SerializeField] Image image;
    [Range(0, 1)] [SerializeField] float transparencyIncreaseOnHit = .1f;
    [SerializeField] float fadeSpeed = .1f;

    AgentHealth playerHealth;

    Color color;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerController>().GetComponent<AgentHealth>();
        playerHealth.OnDamageTaken += PlayerHealth_OnDamageTaken;
        StartCoroutine(FadeIndicator());
        color = image.color;
        color.a = 0;
        image.color = color;
    }

    private void PlayerHealth_OnDamageTaken(DamageSource source, float damage, Vector3 direction)
    {
        if (image.color.a < 1)
        {
            color = image.color;
            color.a += transparencyIncreaseOnHit;
            image.color = color;
        }
    }

    IEnumerator FadeIndicator()
    {
        while (true)
        {
            yield return null;
            if (image.color.a > 0)
            {
                color = image.color;
                color.a -= fadeSpeed * Time.deltaTime;
                image.color = color;
            }
        }
    }
}
