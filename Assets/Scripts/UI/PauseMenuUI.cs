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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
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
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu.SetActive(false);
        Time.timeScale = 1f;
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
