using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] string impactEffectName;
    [SerializeField] LayerMask mask;

    float damage;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void Update()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, speed * Time.deltaTime, mask, QueryTriggerInteraction.Ignore))
        {
            PoolManager.Instance.SpawnObjectWithLifetime(impactEffectName, rayHit.point, transform.rotation, 5f);
            AgentHealth health = rayHit.collider.GetComponentInParent<AgentHealth>();
            if (health)
            {
                health.Damage(damage);
            }
            gameObject.SetActive(false);
        }
        else
        {
            transform.position += speed * Time.deltaTime * transform.forward;
        }
    }

}
