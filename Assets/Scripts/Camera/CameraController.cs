using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static float mouseSensitivityGlobal = 1;

    [SerializeField] Transform head;
    [SerializeField] Transform playerLookTarget;
    public float mouseSensitivity = 1f;
    public Vector3 cameraOffset = new Vector3(0, .2f, .1f);
    public float aimFOVChange = 10f;
    public float fovChangeSpeed = 5f;

    float xRotation;
    Camera mainCam;
    AgentAction playerAction;
    float targetFOV;
    float defaultFOV;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainCam = Camera.main;
        targetFOV = mainCam.fieldOfView;
        defaultFOV = mainCam.fieldOfView;
        playerAction = FindObjectOfType<PlayerController>().GetComponent<AgentAction>();
        playerAction.OnStateChange += PlayerAction_OnStateChange;
    }

    private void PlayerAction_OnStateChange(ActionState state)
    {
        if (state == ActionState.Aim || state == ActionState.AimAttack)
        {
            targetFOV = defaultFOV - aimFOVChange;
        }
        else
        {
            targetFOV = defaultFOV;
        }
    }

    float mouseX;
    float mouseY;
    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity * mouseSensitivityGlobal;
        mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity * mouseSensitivityGlobal;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 85f);

        transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y + mouseX, transform.rotation.eulerAngles.z);
        transform.position = head.position + head.localToWorldMatrix.MultiplyVector(cameraOffset);
        playerLookTarget.position = transform.position + 10 * transform.forward;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
    }
}