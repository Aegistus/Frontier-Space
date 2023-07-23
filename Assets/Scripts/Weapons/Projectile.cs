using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 50f;

    float damage;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }
}
