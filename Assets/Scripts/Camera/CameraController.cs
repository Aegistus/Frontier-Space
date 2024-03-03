using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public static float mouseSensitivityGlobal = 1;

    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform head;
    [SerializeField] Transform playerLookTarget;
    [SerializeField] float regularSensitivity = 1f;
    public Vector3 cameraOffset = new Vector3(0, .2f, .1f);
    public float aimFOVChange = 10f;
    public float fovChangeSpeed = 5f;

    float xRotation;
    Camera mainCam;
    AgentAction playerAction;
    AgentEquipment equipment;
    CameraShake camShake;
    float targetFOV;
    float defaultFOV;
    float currentMouseSensitivity;
    float aimedSensitivity;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainCam = Camera.main;
        targetFOV = mainCam.fieldOfView;
        defaultFOV = mainCam.fieldOfView;
        currentMouseSensitivity = regularSensitivity;
        aimedSensitivity = regularSensitivity / 3;
        playerAction = FindObjectOfType<PlayerController>().GetComponent<AgentAction>();
        playerAction.OnStateChange += PlayerAction_OnStateChange;
        equipment = playerAction.GetComponent<AgentEquipment>();
        equipment.OnWeaponChange += Equipment_OnWeaponChange;
        camShake = GetComponentInParent<CameraShake>();
    }

    private void Equipment_OnWeaponChange()
    {
        equipment.CurrentWeaponAttack.OnRecoil.AddListener(ScreenShake);
    }

    private void PlayerAction_OnStateChange(Type state)
    {
        if (state == typeof(AgentAction.AimState) || state == typeof(AgentAction.AimAttackState))
        {
            if (equipment.CurrentWeaponAttack is RangedWeaponAttack)
            {
                targetFOV = defaultFOV - ((RangedWeaponAttack)equipment.CurrentWeaponAttack).AimFOVChange;
                currentMouseSensitivity = aimedSensitivity;
            }
        }
        else
        {
            targetFOV = defaultFOV;
            currentMouseSensitivity = regularSensitivity;
        }
    }

    float mouseX;
    float mouseY;
    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * currentMouseSensitivity * mouseSensitivityGlobal;
        mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * currentMouseSensitivity * mouseSensitivityGlobal;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 85f);

        cameraHolder.position = head.position + head.localToWorldMatrix.MultiplyVector(cameraOffset);
        cameraHolder.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y + mouseX, transform.rotation.eulerAngles.z);
        playerLookTarget.position = cameraHolder.position + 10 * cameraHolder.forward;
        cameraHolder.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }

    void ScreenShake()
    {
        camShake.StartShake(equipment.CurrentWeaponAttack.camShakeProperties);
    }
}