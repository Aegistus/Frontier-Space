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
    [Header("Aiming")]
    [SerializeField] Vector3 aimingOffset;
    [SerializeField] Vector3 aimingRotation;
    [Header("Reloading")]
    [SerializeField] Vector3 reloadOffset;
    [SerializeField] Vector3 reloadRotation;
    [Header("Crouching")]
    [SerializeField] Vector3 crouchingOffset;
    [SerializeField] Vector3 crouchingRotation;

    public Vector3 IdleOffset => idleOffset;
    public Vector3 IdleRotation => idleRotation;
    public Vector3 RunningOffset => runningOffset;
    public Vector3 RunningRotation => runningRotation;
    public Vector3 AimingOffset => aimingOffset;
    public Vector3 AimingRotation => aimingRotation;
    public Vector3 ReloadOffset => reloadOffset;
    public Vector3 ReloadRotation => reloadRotation;
    public Vector3 CrouchingOffset => crouchingOffset;
    public Vector3 CrouchingRotation => crouchingRotation;


    public Transform RightHandPosition => rightHandPosition;
    public Transform LeftHandPosition => leftHandPosition;
}
