using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject settings;

    private void Awake()
    {
        menu.SetActive(false);
        settings.SetActive(false);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                PauseGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menu.SetActive(true);
        Time.timeScale = 0f;
        SoundManager.Instance.PauseAllSounds();
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu.SetActive(false);
        Time.timeScale = 1f;
        SoundManager.Instance.UnPauseAllSounds();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
        menu.SetActive(false);
    }

    public void CloseSettings()
    {
        settings.SetActive(false);
        menu.SetActive(true);
    }

}
