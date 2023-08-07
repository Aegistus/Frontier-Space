using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] string impactEffectName;

    float damage;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider collider)
    {
        PoolManager.Instance.SpawnObjectWithLifetime(impactEffectName, transform.position, transform.rotation, 5f);
        gameObject.SetActive(false);
    }

}
