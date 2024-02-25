using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPreviewUI : MonoBehaviour
{
    [SerializeField] string levelName = "";

    LevelSelectUI levelSelectUI;

    private void Start()
    {
        levelSelectUI = GetComponentInParent<LevelSelectUI>();
    }

    public void SetSelectedLevel()
    {
        levelSelectUI.SelectedLevel = levelName;
    }
}
