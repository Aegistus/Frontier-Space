using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : MonoBehaviour
{
    [SerializeField] Transform rightHandPosition;
    [SerializeField] Transform leftHandPosition;
    [Header("Idle")]
    [Tooltip("How far away from the person should the object be held?")]
    [SerializeField] Vector3 idleOffset;
    [SerializeField] Vector3 idleRotation;
    [Header("Running")]
    [SerializeField] Vector3 runningOffset;
    [SerializeField] Vector3 runningRotation;

    public Vector3 IdleOffset => idleOffset;
    public Vector3 IdleRotation => idleRotation;
    public Vector3 RunningOffset => runningOffset;
    public Vector3 RunningRotation => runningRotation;
    public Transform RightHandPosition => rightHandPosition;
    public Transform LeftHandPosition => leftHandPosition;
}
