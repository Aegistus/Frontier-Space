using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] GameObject flashlight;

    PlayerController controller;

    private void Awake()
    {
        controller = FindObjectOfType<PlayerController>();
        flashlight.SetActive(false);
    }

    private void Update()
    {
        if (controller.ToggleFlashlight)
        {
            if (flashlight.activeInHierarchy)
            {
                flashlight.SetActive(false);
            }
            else
            {
                flashlight.SetActive(true);
            }
        }
    }
}
