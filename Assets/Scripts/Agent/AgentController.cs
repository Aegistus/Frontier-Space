using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentController : MonoBehaviour
{
    public bool Forwards { get; protected set; }
    public bool Backwards { get; protected set; }
    public bool Left { get; protected set; }
    public bool Right { get; protected set; }
    public bool Jump { get; protected set; }
    public bool Run { get; protected set; }
    public bool Crouch { get; protected set; }
    public bool NoMovementInput => !Forwards && !Backwards && !Left && !Right;
    public bool MovementInput => Forwards || Backwards || Left || Right;
    public Transform Target { get; protected set; }

    public abstract void FindNewTarget();

    public Vector3 GetMovementInput()
    {
        Vector3 input = Vector3.zero;
        if (Forwards)
        {
            input += Vector3.forward;
        }
        if (Backwards)
        {
            input += Vector3.back;
        }
        if (Left)
        {
            input += Vector3.left;
        }
        if (Right)
        {
            input += Vector3.right;
        }
        input.Normalize();
        return input;
    }
}