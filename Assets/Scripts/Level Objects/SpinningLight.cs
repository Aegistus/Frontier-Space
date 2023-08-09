using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLight : MonoBehaviour
{
    [SerializeField] Transform[] lights;
    [SerializeField] float spinSpeed = 20f;
    [SerializeField] bool on = true;

    public void TurnOn()
    {
        on = true;
    }

    public void TurnOff()
    {
        on = false;
    }

    private void Update()
    {
        if (on)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].Rotate(0, spinSpeed * Time.deltaTime, 0, Space.Self);
            }
        }
    }
}
