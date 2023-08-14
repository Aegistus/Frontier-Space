using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_Text sensitivityDisplay;

    private void Start()
    {
        sensitivitySlider.value = CameraController.mouseSensitivityGlobal;
        sensitivityDisplay.text = CameraController.mouseSensitivityGlobal.ToString();
    }

    public void UpdateSensitivityDisplay()
    {
        sensitivityDisplay.text = sensitivitySlider.value.ToString();
    }

    public void SaveSettings()
    {
        CameraController.mouseSensitivityGlobal = sensitivitySlider.value;
    }
}
