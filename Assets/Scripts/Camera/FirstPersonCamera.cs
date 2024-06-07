using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : PlayerCamera
{
    [SerializeField] private Transform cameraRoot;
    
    public override void Move()
    {
        var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.smoothDeltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.smoothDeltaTime;

        transform.position = cameraRoot.position;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.MoveRotation(playerBody.rotation * Quaternion.Euler(0f, mouseX, 0f)); //Vector3.up, mouseX
    }

    public override Camera GetCamera()
    {
        return playerCamera;
    }
}
