using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseInput : MonoBehaviour
{
    [Header ("Events")]
    public Dictionary<string, Action> OnButtonDown;
    public Dictionary<string, Action> OnButtonUp;
    public Action<GameObject> OnSelectedObjectChange;

    [Header ("Key Information")]
    private Dictionary<string, int> buttonAssignment;
    private Dictionary<int, bool> buttonStates;

    [Header ("Raycast Settings")]
    private Camera playerCamera;
    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;

    [Header ("Object Selection")]
    [SerializeField] private GameObject selectedObject;

    private void Awake ()
    {
        buttonStates = new Dictionary<int, bool>();
        buttonAssignment = new Dictionary<string, int>();
        buttonAssignment = InputManager.defaultMouseInput.Zip(InputManager.defaultInputIndexes, (k, v) => new { Key = k, Value = v })
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        UpdateButtonStateDictionary();
        UpdateEventDictionaries();
    }

    private void Start ()
    {
        playerCamera = CameraManager.GetCurrentCamera();
    }

    private void Update ()
    {
        foreach (KeyValuePair<string, int> button in buttonAssignment)
        {
            bool previousState = buttonStates[button.Value];

            buttonStates[button.Value] = Input.GetMouseButton(button.Value);

            if (buttonStates[button.Value] && !previousState)
            {
                OnButtonDown[button.Key]?.Invoke();
            }
            else if (!buttonStates[button.Value] && previousState) 
            {
                OnButtonUp[button.Key]?.Invoke();
            }
        }

        CheckForObjects();
    } 

    private void CheckForObjects ()
    {
        Ray ray = playerCamera.ScreenPointToRay(playerCamera.pixelRect.center);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            GameObject obj = hit.collider.transform.parent.gameObject;

            if (obj == selectedObject) return;
            
            selectedObject = obj;

            OnSelectedObjectChange?.Invoke(selectedObject);
        }
        else if (!ReferenceEquals(selectedObject, null))
        {
            selectedObject = null;
            OnSelectedObjectChange?.Invoke(selectedObject);
        }
    }

    public void CheckForObjectRemoval (GameObject obj)
    {
        if (obj != selectedObject) return;
        
        selectedObject = null;
        OnSelectedObjectChange?.Invoke(selectedObject);
    }

    private void UpdateCamera (Camera camera)
    {
        playerCamera = camera;
    }

    public void AssignButtons (string buttonName, int buttonKey)
    {
        buttonAssignment.Add(buttonName, buttonKey);
        UpdateButtonStateDictionary ();
    }

    public void AssignButtons (Dictionary<string, int> buttonAssignment)
    {
        this.buttonAssignment = buttonAssignment;
        UpdateButtonStateDictionary ();
    }

    private void UpdateButtonStateDictionary () 
    {
        buttonStates = new Dictionary<int, bool>();

        foreach (KeyValuePair<string, int> button in buttonAssignment){
            buttonStates.Add(button.Value, false);
        }
    }

    private void UpdateEventDictionaries () 
    {
        OnButtonDown = new Dictionary<string, Action>();
        OnButtonUp = new Dictionary<string, Action>();

        foreach (var action in InputManager.defaultMouseInput){
            OnButtonDown.Add(action, () => {});
            OnButtonUp.Add(action, () => {});
        }
    }

    public GameObject GetSelectedObject () { return selectedObject; }

    public bool GetButtonState (int key){
        return buttonStates[key];
    }
}
