using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteUI : MonoBehaviour
{
    static NoteUI Instance { get; set; }

    [SerializeField] GameObject menu;
    [SerializeField] TMP_Text noteContent;

    bool inMenu = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        menu.SetActive(false);
    }

    public static void DisplayNote(string contents)
    {
        Instance.noteContent.text = contents;
        Instance.menu.SetActive(true);
        Time.timeScale = 0;
        Instance.inMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void HideNote(bool hideCursor)
    {
        Instance.menu.SetActive(false);
        Instance.inMenu = false;
        Time.timeScale = 1;
        if (hideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (inMenu)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                HideNote(false);
            }
        }
    }
}
