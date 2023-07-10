using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AgentController
{

    private void Update()
    {
        Forwards = Input.GetKey(KeyCode.W);
        Backwards = Input.GetKey(KeyCode.S);
        Left = Input.GetKey(KeyCode.A);
        Right = Input.GetKey(KeyCode.D);
        Jump = Input.GetKey(KeyCode.Space);
    }


    public override void FindNewTarget()
    {
        
    }
}
