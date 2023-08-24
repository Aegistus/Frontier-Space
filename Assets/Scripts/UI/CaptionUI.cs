using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CaptionUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject parentObject;
    [SerializeField] float tipShowTime = 6f;

    private void Start()
    {
        DialogueManager manager = FindObjectOfType<DialogueManager>();
        if (manager != null)
        {
            manager.OnDialogueStart += ShowCaptions;
            manager.OnDialogueEnd += HideCaptions;
        }
        HideCaptions();
    }

    private void HideCaptions()
    {
        parentObject.SetActive(false);
    }

    private void ShowCaptions(string caption)
    {
        text.text = caption;
        parentObject.SetActive(true);
    }

    public void ShowTip(string caption)
    {
        text.text = caption;
        parentObject.SetActive(true);
        Invoke(nameof(HideCaptions), tipShowTime);
    }
}
