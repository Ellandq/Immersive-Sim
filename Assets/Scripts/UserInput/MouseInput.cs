using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseInput : MonoBehaviour
{
    [Header ("Events")]
    private Dictionary<string, Action<ButtonState>> onInputAction;
    private Action<EntityInteraction> onSelectedObjectChange;

    [Header ("Key Information")]
    private Dictionary<string, int> buttonAssignment;
    private Dictionary<int, bool> buttonStates;

    [Header ("Raycast Settings")]
    private Camera playerCamera;
    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;

    [Header ("Object Selection")]
    [SerializeField] private EntityInteraction selectedObject;

    private void Awake ()
    {
        buttonStates = new Dictionary<int, bool>();
        buttonAssignment = new Dictionary<string, int>();
        buttonAssignment = InputManager.defaultMouseInput.Zip(InputManager.defaultInputIndexes, (k, v) => new { Key = k, Value = v })
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        UpdateButtonStateDictionary();
        UpdateEventDictionaries();
    }

    public void SetUp ()
    {
        playerCamera = CameraManager.GetCurrentCamera();
    }

    private void OnApplicationQuit()
    {
        onInputAction.Clear();
        onSelectedObjectChange = null;
    }

    private void Update ()
    {
        foreach (var button in buttonAssignment)
        {
            var previousState = buttonStates[button.Value];

            buttonStates[button.Value] = Input.GetMouseButton(button.Value);

            switch (buttonStates[button.Value])
            {
                case true when !previousState:
                    onInputAction[button.Key]?.Invoke(ButtonState.Down);
                    break;
                case false when previousState:
                    onInputAction[button.Key]?.Invoke(ButtonState.Up);
                    break;
            }
        }

        CheckForObjects();
    } 

    private void CheckForObjects ()
    {
        var ray = playerCamera.ScreenPointToRay(playerCamera.pixelRect.center);

        if (Physics.Raycast(ray, out var hit, range, layerMask))
        {
            var obj = hit.collider.transform.gameObject;

            if (!obj.CompareTag("Interactable")) return;
            var interactor = obj.GetComponent<EntityInteraction>();
            
            if (interactor == selectedObject) return;
            selectedObject = interactor;

            onSelectedObjectChange?.Invoke(selectedObject);
        }
        else if (!ReferenceEquals(selectedObject, null))
        {
            selectedObject = null;
            onSelectedObjectChange?.Invoke(selectedObject);
        }
    }

    public void CheckForObjectRemoval (EntityInteraction obj)
    {
        if (obj != selectedObject) return;
        
        selectedObject = null;
        onSelectedObjectChange?.Invoke(selectedObject);
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

        foreach (var button in buttonAssignment){
            buttonStates.Add(button.Value, false);
        }
    }

    private void UpdateEventDictionaries () 
    {
        onInputAction = new Dictionary<string, Action<ButtonState>>();

        foreach (var action in InputManager.defaultMouseInput){
            onInputAction.Add(action, (state) => {});
        }
    }

    public void AddListenerOnInputAction (Action<ButtonState> actionToAdd, string key) { onInputAction[key] += actionToAdd; }
    
    public void AddListenerOnObjectChange (Action<EntityInteraction> actionToAdd) { onSelectedObjectChange += actionToAdd; }

    public EntityInteraction GetSelectedObject () { return selectedObject; }

    public bool GetButtonState (int key){
        return buttonStates[key];
    }
    
    
}
