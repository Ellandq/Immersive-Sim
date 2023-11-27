using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    public Action<Camera> onCameraChange;

    [Header ("Camera References")]
    private Dictionary<ActiveCamera, Camera> cameras;
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Camera cutsceneCamera;

    [Header ("Object References")]
    [SerializeField] private Transform playerBody;

    [Header ("Camera Information")]
    [SerializeField] private ActiveCamera activeCamera;
    [SerializeField] private float firstPersonCameraSensitivity = 500f;
    [SerializeField] private float thirdPersonCameraSensitivity = 80f;
    private float xRotation = 0f;
    private bool cameraMovementEnabled = false;

    private void Awake ()
    {
        Instance = this;
        cameraMovementEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        cameras = new Dictionary<ActiveCamera, Camera>
        {
            { ActiveCamera.FirstPerson, firstPersonCamera },
            { ActiveCamera.ThirdPerson, thirdPersonCamera },
            { ActiveCamera.Cutscene, cutsceneCamera }   
        };
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
        foreach (ActiveCamera cam in Enum.GetValues(typeof(ActiveCamera)))
        {
            if (activeCamera == cam) cameras[cam].enabled = true;
            else cameras[cam].enabled = false;
        }
        onCameraChange?.Invoke(cameras[activeCamera]);
    }

    public static Camera GetCurrentCamera () { return Instance.cameras[Instance.activeCamera]; }
}

public enum ActiveCamera {
    FirstPerson, ThirdPerson, Cutscene
}
