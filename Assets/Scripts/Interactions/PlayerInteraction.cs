using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Player player;

    private EntityInteraction selectedObject;

    public void Start ()
    {
        var inputHandle = InputManager.GetInputHandle();

        inputHandle.OnButtonDown["Interact"] += InteractWithObject;

        var mouseHandle = InputManager.GetMouseHandle();

        mouseHandle.OnSelectedObjectChange += UpdateSelectedObject;
    }

    private void InteractWithObject ()
    {
        if (selectedObject == null) return;
        
        selectedObject.Interact(player);
    }

    private void UpdateSelectedObject(GameObject obj)
    {
        if (ReferenceEquals(obj, null))
        {
            selectedObject = null;
            return;
        }
        selectedObject = obj.GetComponent<EntityInteraction>();
    }
}
