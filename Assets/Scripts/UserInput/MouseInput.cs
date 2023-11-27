using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseInput : MonoBehaviour
{
    [Header ("Events")]
    public Dictionary<string, Action> onButtonDown;
    public Dictionary<string, Action> onButtonUp;

    [Header ("Key Information")]
    private Dictionary<string, int> buttonAssignment;
    private Dictionary<int, bool> buttonStates;

    private void Awake ()
    {
        buttonStates = new Dictionary<int, bool>();
        buttonAssignment = new Dictionary<string, int>();
        buttonAssignment = InputManager.defaultMouseInput.Zip(InputManager.defaultInputIndexes, (k, v) => new { Key = k, Value = v })
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        UpdateButtonStateDictionary();
        UpdateEventDictionaries();
    }

    private void Update ()
    {
        foreach (KeyValuePair<string, int> button in buttonAssignment)
        {
            bool previousState = buttonStates[button.Value];

            buttonStates[button.Value] = Input.GetMouseButton(button.Value);

            if (buttonStates[button.Value] && !previousState)
            {
                onButtonDown[button.Key]?.Invoke();
            }
            else if (!buttonStates[button.Value] && previousState) 
            {
                onButtonUp[button.Key]?.Invoke();
            }
        }
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
        onButtonDown = new Dictionary<string, Action>();
        onButtonUp = new Dictionary<string, Action>();

        foreach (string action in InputManager.defaultMouseInput){
            onButtonDown.Add(action, () => {});
            onButtonUp.Add(action, () => {});
        }
    }

    public Action GetOnButtonDownEvent (string input){
        return onButtonDown[input];
    }

    public Action GetOnButtonUpEvent (string input){
        return onButtonUp[input];
    }

    public bool GetButtonState (int key){
        return buttonStates[key];
    }
}
