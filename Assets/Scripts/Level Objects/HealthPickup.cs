using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IInteractable
{
    [SerializeField] float percentHeal = .5f;

    public string Description => "[E] Use Health Pack";

    public void Interact(GameObject interactor)
    {
        AgentHealth health = interactor.GetComponent<AgentHealth>();
        if (health)
        {
            health.Heal(health.MaxHealth * percentHeal);
        }
        gameObject.SetActive(false);
    }
}
