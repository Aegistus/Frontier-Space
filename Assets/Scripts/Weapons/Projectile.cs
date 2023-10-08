using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] string impactEffectName;
    [SerializeField] LayerMask mask;

    float damage;
    DamageSource source;

    public void SetDamage(float damage, DamageSource source)
    {
        this.damage = damage;
        this.source = source;
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
                health.Damage(damage, source);
                SoundManager.Instance.PlaySoundAtPosition("Impact_Flesh", transform.position);
            }
            else
            {
                SoundManager.Instance.PlaySoundAtPosition("Impact_Metal", transform.position);
            }
            gameObject.SetActive(false);
        }
        else
        {
            transform.position += speed * Time.deltaTime * transform.forward;
        }
    }

}
