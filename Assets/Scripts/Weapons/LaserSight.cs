using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    [SerializeField] LayerMask hitLayers;
    [SerializeField] float maxDistance = 200f;

    LineRenderer lineRend;
    bool on = true;
    RaycastHit rayHit;

    private void Awake()
    {
        lineRend = GetComponentInChildren<LineRenderer>();
    }

    public void TurnOn()
    {
        lineRend.gameObject.SetActive(true);
        on = true;
    }

    public void TurnOff()
    {
        lineRend.gameObject.SetActive(false);
        on = false;
    }

    private void Update()
    {
        if (on)
        {
            if (Physics.Raycast(transform.position, transform.forward, out rayHit, maxDistance, hitLayers))
            {
                lineRend.SetPosition(1, transform.InverseTransformPoint(rayHit.point));
            }
            else
            {
                lineRend.SetPosition(1, transform.InverseTransformPoint(transform.forward * maxDistance));
            }

        }
    }
}
