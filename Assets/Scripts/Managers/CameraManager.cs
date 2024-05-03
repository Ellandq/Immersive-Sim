using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class CameraManager : MonoBehaviour, IManager
{
    private static CameraManager Instance;

    public Action<Camera> OnCameraChange;

    [Header ("Camera References")]
    private Dictionary<ActiveCamera, PlayerCamera> cameras;
    [SerializeField] private PlayerCamera cutsceneCamera;

    [Header ("Object References")]
    private Transform playerBody;

    [Header ("Camera Information")]
    [SerializeField] private ActiveCamera activeCamera;
    [SerializeField] private float firstPersonCameraSensitivity = 500f;
    [SerializeField] private float thirdPersonCameraSensitivity = 80f;
    private bool cameraMovementEnabled = false;

    private void Awake ()
    {
        Instance = this;
    }

    public void SetUp()
    {
        cameraMovementEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        var player = PlayerManager.GetPlayer();
        playerBody = player.transform;

        cameras = new Dictionary<ActiveCamera, PlayerCamera>
        {
            { ActiveCamera.FirstPerson, player.GetFirstPersonCamera() },
            { ActiveCamera.ThirdPerson, player.GetThirdPersonCamera() },
            { ActiveCamera.Cutscene, cutsceneCamera }   
        };
        
        cameras[ActiveCamera.FirstPerson].Initialize(firstPersonCameraSensitivity, playerBody);
        cameras[ActiveCamera.ThirdPerson].Initialize(firstPersonCameraSensitivity, playerBody);
        cutsceneCamera.Initialize(0f, playerBody);
    }

    private void Update ()
    {
        if (!cameraMovementEnabled) return;

        cameras[activeCamera].Move();
    }

    public void SwitchCamera (ActiveCamera activeCamera)
    {
        this.activeCamera = activeCamera;
        foreach (ActiveCamera cam in Enum.GetValues(typeof(ActiveCamera)))
        {
            cameras[cam].Enabled = activeCamera == cam;
        }
        OnCameraChange?.Invoke(cameras[activeCamera].GetCamera());
    }

    public static Camera GetCurrentCamera () { return Instance.cameras[Instance.activeCamera].GetCamera(); }

    public static CameraManager GetInstance() { return Instance; }
}

public enum ActiveCamera {
    FirstPerson, ThirdPerson, Cutscene
}
