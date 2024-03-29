using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FirstPersonCamera : PlayerCamera
{
    [SerializeField] private Camera playerCamera;
    public override void Move()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up, mouseX);
    }

    public override Camera GetCamera()
    {
        return playerCamera;
    }
}
