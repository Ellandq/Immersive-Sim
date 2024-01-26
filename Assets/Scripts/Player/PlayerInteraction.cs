using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Player player;

    private GameObject selectedObject;

    public void Start ()
    {
        var inputHandle = InputManager.GetInputHandle();

        inputHandle.OnButtonDown["Interact"] += InteractWithObject;

        var mouseHandle = InputManager.GetMouseHandle();

        mouseHandle.OnSelectedObjectChange += UpdateSelectedObject;
    }

    public void InteractWithObject ()
    {
        if (selectedObject == null) return;

        ItemHolder itemHolder = selectedObject.GetComponentInParent<ItemHolder>();

        if (itemHolder == null) return;

        Inventory inv = player.GetInventory();

        inv.AddItems(itemHolder.GetItems());

        Destroy(itemHolder.gameObject);
    }

    private void UpdateSelectedObject (GameObject obj) { selectedObject = obj; }
}
