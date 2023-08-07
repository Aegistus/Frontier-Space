using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Transform healthBarTransform;
    [SerializeField] Transform armorBarTransform;

    AgentHealth agentHealth;


    private void Start()
    {
        agentHealth = FindObjectOfType<PlayerController>().GetComponent<AgentHealth>();
        agentHealth.OnHealthChange += UpdateHealthBar;
        agentHealth.OnArmorChange += UpdateArmorBar;
        UpdateHealthBar();
        UpdateArmorBar();
    }

    private void UpdateHealthBar()
    {
        float percentHealth = agentHealth.CurrentHealth / agentHealth.MaxHealth;
        healthBarTransform.localScale = new Vector2(percentHealth, 1);
    }

    private void UpdateArmorBar()
    {
        float percentArmor = agentHealth.CurrentArmor / agentHealth.MaxArmor;
        armorBarTransform.localScale = new Vector2(percentArmor, 1);
    }

}