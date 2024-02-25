using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] GameObject holder;

    public string SelectedLevel { get; set; }

    private void Start()
    {
        holder.SetActive(false);
    }

    public void OpenMenu()
    {
        holder.SetActive(true);
    }

    public void CloseMenu()
    {
        holder.SetActive(false);
    }

    public void PlaySelectedLevel()
    {
        SceneManager.LoadScene(SelectedLevel);
    }
}
