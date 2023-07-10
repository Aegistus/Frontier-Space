using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add more animator states here as needed. The enum name must match the state name in the animator controller!
/// </summary>
public enum FullBodyAnimState
{
    Idle, WalkForwards, WalkLeft, WalkRight, WalkBackwards
}

public enum UpperBodyAnimState
{
    None, Punch
}