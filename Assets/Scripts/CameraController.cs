using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] Transform playerLookTarget;
    public float mouseSensitivity = 1f;
    public Vector3 cameraOffset = new Vector3(0, .2f, .1f);

    float xRotation;
    Camera mainCam;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainCam = Camera.main;
    }

    float mouseX;
    float mouseY;
    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 85f);

        transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y + mouseX, transform.rotation.eulerAngles.z);
        transform.position = head.position + head.localToWorldMatrix.MultiplyVector(cameraOffset);
        playerLookTarget.position = transform.position + 10 * transform.forward;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
}