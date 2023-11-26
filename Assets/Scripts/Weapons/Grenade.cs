using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject particleEffect;
    [SerializeField] LayerMask targetableLayers;
    [SerializeField] float maxDamage = 100f;
    [SerializeField] float fuseTime = 3f;
    [SerializeField] float blastRadius = 5f;

    DamageSource source;

    private void Start()
    {
        //Arm();
    }

    public void Arm()
    {
        Arm(DamageSource.Environment);
    }

    public void Arm(DamageSource source)
    {
        this.source = source;
        StartCoroutine(Fuse());
        particleEffect.SetActive(true);
        SoundManager.Instance.PlaySoundAtPosition("Grenade_Arm", transform.position);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    IEnumerator Fuse()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        SoundManager.Instance.PlaySoundAtPosition("Grenade_Explosion", transform.position);
        PoolManager.Instance.SpawnObject("Grenade_Explosion", transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, targetableLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            AgentHealth health = colliders[i].GetComponent<AgentHealth>();
            if (health != null)
            {
                float distance = Vector3.Distance(transform.position, health.transform.position);
                float damage = maxDamage * Mathf.InverseLerp(blastRadius, 0, distance);
                health.Damage(damage, (transform.position - health.transform.position).normalized, health.transform.position, source);
            }
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        SoundManager.Instance.PlaySoundAtPosition("Grenade_Bounce", transform.position);
    }
}
