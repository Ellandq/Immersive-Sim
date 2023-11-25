using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header ("Camera References")]
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera cutsceneCamera;

    [Header ("Object References")]
    [SerializeField] private Transform playerBody;

    [Header ("Camera Information")]
    [SerializeField] private ActiveCamera activeCamera;
    private float firstPersonCameraSensitivity = 500f;
    private float thirdPersonCameraSensitivity = 80f;
    private float xRotation = 0f;
    private bool cameraMovementEnabled = false;

    private void Start ()
    {
        cameraMovementEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update ()
    {
        if (!cameraMovementEnabled) return;

        switch (activeCamera)
        {
            case ActiveCamera.FirstPerson:
                FirstPersonCameraMovement();
            break;

            case ActiveCamera.ThirdPerson:
                ThirdPersonCameraMovement();
            break;

            default: return;
        }
    }

    private void FirstPersonCameraMovement ()
    {
        float mouseX = Input.GetAxis("Mouse X") * firstPersonCameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * firstPersonCameraSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up, mouseX);
    }

    private void ThirdPersonCameraMovement ()
    {

    }

    public void SwitchCamera (ActiveCamera activeCamera)
    {
        this.activeCamera = activeCamera;
        switch (activeCamera)
        {
            case ActiveCamera.FirstPerson:
                firstPersonCamera.enabled = true;
                thirdPersonCamera.enabled = false;
                cutsceneCamera.enabled = false;
            break;

            case ActiveCamera.ThirdPerson:
                firstPersonCamera.enabled = false;
                thirdPersonCamera.enabled = true;
                cutsceneCamera.enabled = false;
            break;

            case ActiveCamera.Cutscene:
                firstPersonCamera.enabled = false;
                thirdPersonCamera.enabled = false;
                cutsceneCamera.enabled = true;
            break;
        }
    }

    
}

public enum ActiveCamera {
    FirstPerson, ThirdPerson, Cutscene
}
