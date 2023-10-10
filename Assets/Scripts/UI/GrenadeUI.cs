using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeUI : MonoBehaviour
{
    [SerializeField] GameObject[] grenadeIcons;

    AgentEquipment playerEquipment;

    private void Awake()
    {
        for (int i = 0; i < grenadeIcons.Length; i++)
        {
            grenadeIcons[i].SetActive(false);
        }
    }

    private void Start()
    {
        playerEquipment = FindObjectOfType<PlayerController>().GetComponent<AgentEquipment>();
        playerEquipment.OnGrenadeCountChange += UpdateGrenadeCount;
    }

    private void UpdateGrenadeCount(int count)
    {
        int i = 0;
        for (; i < grenadeIcons.Length && i < count; i++)
        {
            grenadeIcons[i].SetActive(true);
        }
        for (; i < grenadeIcons.Length; i++)
        {
            grenadeIcons[i].SetActive(false);
        }
    }
}
