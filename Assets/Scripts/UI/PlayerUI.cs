using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<PlayerController>().GetComponent<AgentHealth>().OnAgentDeath += () => gameObject.SetActive(false);
    }
}
