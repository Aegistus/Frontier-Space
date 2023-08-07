using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentHealth : MonoBehaviour
{
    public event Action OnArmorChange;
    public event Action OnHealthChange;
    public event Action OnAgentDeath;

    public float CurrentArmor => currentArmor;
    public float CurrentHealth => currentHealth;
    public float MaxArmor => maxArmor;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    bool isDead = false;

    [SerializeField] bool allowArmorRegen = false;
    [SerializeField] float armorRegenDelay = 2f;
    [SerializeField] float armorRegenRate = 20f;
    [SerializeField] float maxArmor = 100;
    [SerializeField] float maxHealth = 100f;
    float currentArmor;
    float currentHealth;
    float delayTimer;

    int hitSoundID;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
    }

    //private void Start()
    //{
    //    hitSoundID = SoundManager.Instance.GetSoundID("Agent_Hit");
    //}

    private void Update()
    {
        if (delayTimer == 0 && currentArmor < maxArmor)
        {
            currentArmor += armorRegenRate * Time.deltaTime;
            if (currentArmor > maxArmor)
            {
                currentArmor = maxArmor;
            }
            OnArmorChange?.Invoke();
        }
        else
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer < 0)
            {
                delayTimer = 0;
            }
        }
    }

    public void Damage(float damage)
    {
        if (isDead)
        {
            return;
        }
        if (currentArmor >= damage)
        {
            currentArmor -= damage;
            delayTimer = armorRegenDelay;
            OnArmorChange?.Invoke();
            return;
        }
        else
        {
            damage -= currentArmor;
            currentArmor = 0;
            delayTimer = armorRegenDelay;
            OnArmorChange?.Invoke();
            currentHealth -= damage;
            //SoundManager.Instance.PlaySoundAtPosition(hitSoundID, transform.position);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
                return;
            }
            OnHealthChange?.Invoke();
        }
    }

    void Die()
    {
        isDead = true;
        OnHealthChange?.Invoke();
        OnAgentDeath?.Invoke();
    }
}