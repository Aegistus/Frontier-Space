using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class AgentHealth : MonoBehaviour
{
    // Source, amount, direction
    public event Action<DamageSource, float, Vector3> OnDamageTaken;
    public event Action OnArmorChange;
    public event Action OnHealthChange;
    public event Action OnAgentDeath;

    public float CurrentArmor => currentArmor;
    public float CurrentHealth => currentHealth;
    public float MaxArmor => maxArmor;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    bool isDead = false;

    [SerializeField] LayerMask bloodSplatterLayers;
    [SerializeField] List<DamageSource> damageImmunities;
    [SerializeField] bool allowArmorRegen = false;
    [SerializeField] float armorRegenDelay = 2f;
    [SerializeField] float armorRegenRate = 20f;
    [SerializeField] float maxArmor = 100;
    [SerializeField] float maxHealth = 100f;

    float currentArmor;
    float currentHealth;
    float delayTimer;
    readonly float ragdollDuration = 5f;
    readonly float bloodSplatterReachDistance = 7f;

    Ragdoll ragdoll;
    AgentEquipment equipment;

    int armorRechargeStartID = -1;
    int armorRechargeEndID = -1;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
        ragdoll = GetComponentInChildren<Ragdoll>();
        equipment = GetComponent<AgentEquipment>();
    }

    private void Start()
    {
        armorRechargeStartID = SoundManager.Instance.GetSoundID("Armor_Recharge_Start");
        armorRechargeEndID = SoundManager.Instance.GetSoundID("Armor_Recharge_End");
    }

    private void Update()
    {
        if (allowArmorRegen && currentArmor < maxArmor)
        {
            if (delayTimer == 0)
            {
                currentArmor += armorRegenRate * Time.deltaTime;
                if (currentArmor > maxArmor)
                {
                    currentArmor = maxArmor;
                    //SoundManager.Instance.PlaySoundAtPosition(armorRechargeEndID, transform.position, transform);
                }
                OnArmorChange?.Invoke();
            }
            else
            {
                delayTimer -= Time.deltaTime;
                if (delayTimer < 0)
                {
                    delayTimer = 0;
                    //SoundManager.Instance.PlaySoundAtPosition(armorRechargeStartID, transform.position, transform);
                }
            }
        }
    }

    public void EquipArmor(float armor)
    {
        allowArmorRegen = true;
        maxArmor = armor;
    }

    public bool Damage(float damage, Vector3 direction, Vector3 point, DamageSource source)
    {
        if (isDead)
        {
            return false;
        }
        if (damageImmunities.Contains(source))
        {
            return false;
        }
        damage = DamageArmor(damage);
        DamageHealth(damage, direction, point);
        OnDamageTaken?.Invoke(source, damage, direction);
        return true;
    }

    float DamageArmor(float damage)
    {
        if (currentArmor >= damage)
        {
            currentArmor -= damage;
            damage = 0f;
            delayTimer = armorRegenDelay;
            OnArmorChange?.Invoke();
        }
        else
        {
            damage -= currentArmor;
            currentArmor = 0;
            delayTimer = armorRegenDelay;
            OnArmorChange?.Invoke();
        }
        return damage;
    }

    void DamageHealth(float damage, Vector3 direction, Vector3 point)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Kill();
            return;
        }
        // create blood splatter
        RaycastHit rayHit;
        if (Physics.Raycast(point, direction, out rayHit, bloodSplatterReachDistance, bloodSplatterLayers))
        {
            GameObject bloodSplatter = PoolManager.Instance.SpawnObject("Blood_Splatter", rayHit.point, Quaternion.identity);
            bloodSplatter.transform.LookAt(rayHit.point + rayHit.normal);
        }
        OnHealthChange?.Invoke();
    }

    public void Heal(float healing)
    {
        currentHealth += healing;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChange?.Invoke();
    }

    public void Kill()
    {
        isDead = true;
        ragdoll.EnableRagdoll();
        equipment.DropWeapon();
        OnHealthChange?.Invoke();
        OnAgentDeath?.Invoke();
        StartCoroutine(StopRagdoll());
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        Collider collider = GetComponent<Collider>();
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    IEnumerator StopRagdoll()
    {
        yield return new WaitForSeconds(ragdollDuration);
        ragdoll.DisableRagdoll();
    }
}