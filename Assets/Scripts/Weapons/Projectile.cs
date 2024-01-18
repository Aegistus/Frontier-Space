using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] string impactEffectName;
    [SerializeField] LayerMask mask;
    /// <summary>
    /// The delay between the projectile being shot and the projectile becoming visible. This prevents weird instances where projectiles look like they're
    /// coming out of the side of the gun.
    /// </summary>
    [SerializeField] float invisibilityDelay = .5f;
    [SerializeField] GameObject model;

    float damage;
    DamageSource source;

    private void OnEnable()
    {
        model.SetActive(false);
        Invoke(nameof(BecomeVisible), invisibilityDelay);
    }

    void BecomeVisible()
    {
        model.SetActive(true);
    }

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
                health.Damage(damage, transform.forward, transform.position, source);
                SoundManager.Instance.PlaySoundAtPosition("Impact_Flesh", transform.position);
            }
            else
            {
                SoundManager.Instance.PlaySoundAtPosition("Impact_Metal", transform.position);
                PoolManager.Instance.SpawnObject("Impact_Metal", transform.position, Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360));
                GameObject bulletHole = PoolManager.Instance.SpawnObjectWithLifetime("Bullet_Hole", rayHit.point, Quaternion.identity, 60);
                bulletHole.transform.LookAt(rayHit.point + rayHit.normal);
            }
            gameObject.SetActive(false);
        }
        else
        {
            transform.position += speed * Time.deltaTime * transform.forward;
        }
    }

}
