using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Position")]
    public float amount = .02f;
    public float maxAmount = .06f;
    public float smoothAmount = 6f;
    [Header("Rotation")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 12f;
    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    Vector3 RestPosition { get; set; }
    Quaternion RestRotation { get; set; }
    float inputX;
    float inputY;

    private void Update()
    {
        CalculateSway();
        MoveSway();
        TiltSway();
    }

    void CalculateSway()
    {
        inputX = -Input.GetAxis("Mouse X");
        inputY = -Input.GetAxis("Mouse Y");
    }

    void MoveSway()
    {
        float moveX = Mathf.Clamp(inputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(inputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + RestPosition, Time.deltaTime * smoothAmount);
    }

    void TiltSway()
    {
        float tiltY = Mathf.Clamp(inputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltX = Mathf.Clamp(inputY * rotationAmount, -maxRotationAmount, maxRotationAmount);
        Quaternion finalRotation = Quaternion.Euler(rotationX ? -tiltX : 0, rotationY ? tiltY : 0, rotationZ ? tiltY : 0);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * RestRotation, Time.deltaTime * smoothRotation);
    }
}
