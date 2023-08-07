using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentHealth : MonoBehaviour
{
    public event Action OnDamageTaken;
    public event Action OnAgentDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    bool isDead = false;

    [SerializeField] float maxHealth = 100f;
    float currentHealth;

    int hitSoundID;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        hitSoundID = SoundManager.Instance.GetSoundID("Agent_Hit");
    }

    public void Damage(float damage)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= damage;
        //SoundManager.Instance.PlaySoundAtPosition(hitSoundID, transform.position);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }
        OnDamageTaken?.Invoke();
    }

    void Die()
    {
        isDead = true;
        OnDamageTaken?.Invoke();
        OnAgentDeath?.Invoke();
    }
}