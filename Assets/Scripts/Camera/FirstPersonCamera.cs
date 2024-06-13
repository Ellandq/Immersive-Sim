using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : PlayerCamera
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private Transform cameraFollowObject;
    
    public override void Move(bool canMove)
    {
        transform.position = cameraRoot.position;
        if (!canMove) return;
        
        var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.smoothDeltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.smoothDeltaTime;
        
        var forwardPoint = transform.position + transform.forward * 3.0f;
        cameraFollowObject.position = forwardPoint;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.MoveRotation(playerBody.rotation * Quaternion.Euler(0f, mouseX, 0f));
    }

    public override Camera GetCamera()
    {
        return playerCamera;
    }
}
