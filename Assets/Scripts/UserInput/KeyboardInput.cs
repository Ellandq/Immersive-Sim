using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    [Header ("Events")]
    public Dictionary<string, Action> onButtonDown;
    public Dictionary<string, Action> onButtonUp;

    [Header ("Key Information")]
    private Dictionary<string, KeyCode> buttonAssignment;
    private Dictionary<KeyCode, bool> buttonStates;

    private void Awake ()
    {
        buttonStates = new Dictionary<KeyCode, bool>();
        buttonAssignment = new Dictionary<string, KeyCode>();
        buttonAssignment = InputManager.defaultInputCodes.Zip(InputManager.defaultInputKeys, (k, v) => new { Key = k, Value = v })
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        UpdateEventDictionaries();
    }

    private void Update ()
    {
        foreach (KeyValuePair<string, KeyCode> button in buttonAssignment)
        {
            bool previousState = buttonStates[button.Value];
            buttonStates[button.Value] = Input.GetKey(button.Key);

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

    public void AssignButtons (string buttonName, KeyCode buttonKey)
    {
        buttonAssignment.Add(buttonName, buttonKey);
        UpdateButtonStateDictionary ();
    }

    public void AssignButtons (Dictionary<string, KeyCode> buttonAssignment)
    {
        this.buttonAssignment = buttonAssignment;
        UpdateButtonStateDictionary ();
    }

    private void UpdateButtonStateDictionary () 
    {
        buttonStates = new Dictionary<KeyCode, bool>();

        foreach (KeyValuePair<string, KeyCode> button in buttonAssignment){
            buttonStates.Add(button.Value, Input.GetKey(button.Key));
        }
    }

    private void UpdateEventDictionaries () 
    {
        onButtonDown = new Dictionary<string, Action>();
        onButtonUp = new Dictionary<string, Action>();

        foreach (string action in InputManager.defaultInputCodes){
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

    public bool GetButtonState (KeyCode key){
        return buttonStates[key];
    }
}
