using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject particleEffect;
    [SerializeField] float fuseTime = 3f;
    [SerializeField] float blastRadius = 5f;

    private void Start()
    {
        //Arm();
    }

    public void Arm()
    {
        StartCoroutine(Fuse());
        particleEffect.SetActive(true);
        SoundManager.Instance.PlaySoundAtPosition("Grenade_Arm", transform.position);
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
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        SoundManager.Instance.PlaySoundAtPosition("Grenade_Bounce", transform.position);
    }
}
