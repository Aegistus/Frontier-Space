using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float fuseTime = 3f;
    [SerializeField] float blastRadius = 5f;

    private void Start()
    {
        //Arm();
    }

    public void Arm()
    {
        StartCoroutine(Fuse());
    }

    IEnumerator Fuse()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        print("Explode");
        PoolManager.Instance.SpawnObject("Grenade_Explosion", transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
