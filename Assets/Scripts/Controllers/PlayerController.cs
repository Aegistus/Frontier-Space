using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AgentController
{
    [SerializeField] Transform lookTarget;

    private void Start()
    {
        LookTarget = lookTarget;
    }

    private void Update()
    {
        Forwards = Input.GetKey(KeyCode.W);
        Backwards = Input.GetKey(KeyCode.S);
        Left = Input.GetKey(KeyCode.A);
        Right = Input.GetKey(KeyCode.D);
        Jump = Input.GetKey(KeyCode.Space);
        Run = Input.GetKey(KeyCode.LeftShift);
        Crouch = Input.GetKey(KeyCode.LeftControl);

        Attack = Input.GetMouseButton(0);
        Aim = Input.GetMouseButton(1);
        Interact = Input.GetKeyDown(KeyCode.E);
        Reload = Input.GetKeyDown(KeyCode.R);
        transform.LookAt(LookTarget);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

}
