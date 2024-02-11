using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterboxUI : MonoBehaviour
{
    [SerializeField] GameObject barHolder;

    private void Start()
    {
        GetComponentInChildren<UITextFadeIn>().FadeIn();
    }

    public void EnableLetterbox(bool enabled)
    {
        barHolder.SetActive(enabled);
    }
}
