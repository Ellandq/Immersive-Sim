using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Player player;

    private EntityInteraction selectedObject;
    
    public bool AllowInteraction { get; set; }

    public void Start ()
    {
        var inputHandle = InputManager.GetInputHandle();
        inputHandle.AddListenerOnInputAction(InteractWithObject, "Interact");

        var mouseHandle = InputManager.GetMouseHandle();
        mouseHandle.AddListenerOnObjectChange(UpdateSelectedObject);

        AllowInteraction = true;
    }

    private void InteractWithObject (ButtonState state)
    {
        if (state == ButtonState.Up || selectedObject == null || !AllowInteraction) return;
        
        selectedObject.Interact(player);
    }

    private void UpdateSelectedObject(EntityInteraction obj)
    {
        if (ReferenceEquals(obj, null))
        {
            selectedObject = null;
            return;
        }

        selectedObject = obj;
    }
}
