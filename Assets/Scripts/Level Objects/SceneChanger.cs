using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] string scene;

    public void ChangeScene()
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
