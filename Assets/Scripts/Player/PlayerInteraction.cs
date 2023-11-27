using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Player player;

    private GameObject selectedObject;

    public void Start ()
    {
        PlayerInput inputHandle = InputManager.GetInputHandle();

        inputHandle.onButtonDown["Interact"] += InteractWithObject;

        MouseInput mouseHandle = InputManager.GetMouseHandle();

        mouseHandle.onSelectedObjectChange += UpdateSelectedObject;
    }

    public void InteractWithObject ()
    {
        if (selectedObject == null) return;

        ItemHolder itemHolder = selectedObject.GetComponent<ItemHolder>();

        if (itemHolder == null) return;

        Inventory inv = player.GetInventory();

        inv.AddItems(itemHolder.GetItems());

        Destroy(selectedObject);
    }

    public void UpdateSelectedObject (GameObject obj) { selectedObject = obj; }
}
