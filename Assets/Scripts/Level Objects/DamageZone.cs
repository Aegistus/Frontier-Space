using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] float damagePerSecond = 5f;

    List<AgentHealth> toDamage = new List<AgentHealth>();

    private void OnTriggerEnter(Collider other)
    {
        AgentHealth health = other.GetComponentInParent<AgentHealth>();
        if (health)
        {
            toDamage.Add(health);
        }
    }

    private void Update()
    {
        for (int i = 0; i < toDamage.Count; i++)
        {
            toDamage[i].Damage(damagePerSecond * Time.deltaTime, DamageSource.Environment);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AgentHealth health = other.GetComponentInParent<AgentHealth>();
        if (health)
        {
            toDamage.Remove(health);
        }
    }
}
