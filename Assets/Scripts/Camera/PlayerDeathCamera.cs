using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathCamera : MonoBehaviour
{
    [SerializeField] Camera deathCamera;
    [SerializeField] float deathCamLength = 3f;

    private void Start()
    {
        FindObjectOfType<PlayerController>().GetComponent<AgentHealth>().OnAgentDeath += PlayerDeathCamera_OnAgentDeath;
        deathCamera.gameObject.SetActive(false);
    }

    private void PlayerDeathCamera_OnAgentDeath()
    {
        transform.SetParent(null);
        deathCamera.gameObject.SetActive(true);
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(deathCamLength);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
