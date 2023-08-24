using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] string scene;
    [SerializeField] bool skippableScene = false;

    private void Update()
    {
        if (skippableScene && Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
