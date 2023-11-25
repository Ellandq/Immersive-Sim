using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header ("Input Handles")]
    [SerializeField] private KeyboardInput keyboardInputHandle;
    [SerializeField] private MouseInput mouseInputHandle;

    public void Awake () 
    { 
        if  (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }else{
            Destroy(gameObject);
        }
    }

    public static KeyboardInput GetKeyboardHandle (){
        return Instance.GetKeyboardInputHandle();
    }

    private KeyboardInput GetKeyboardInputHandle (){
        return keyboardInputHandle;
    }

    public static MouseInput GetMouseHandle (){
        return Instance.GetMouseInputHandle();
    }

    private MouseInput GetMouseInputHandle (){
        return mouseInputHandle;
    }

    public static List<string> defaultInputCodes = new List<string>(){
        "Move Left", "Move Right", "Move Forwards", "Move Backwards", 
        "Jump", "Sprint", "Crouch", "Walk", "Interact", "Inventory", "Character Info", 
        "Item Slot 1", "Item Slot 2", "Item Slot 3", "Item Slot 4", "Item Slot 5"
    };

    public static List<KeyCode> defaultInputKeys = new List<KeyCode>(){
        KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,
        KeyCode.Space, KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.E, KeyCode.Tab, KeyCode.C,
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5
    };
}

