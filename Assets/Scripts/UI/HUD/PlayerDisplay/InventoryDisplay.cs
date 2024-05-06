using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryDisplay : UI_Component
{
    [Header("Display Information")]
    [SerializeField] private Vector3 firstRowPosition;
    private Inventory playerInventory;
    private ItemType currentDisplayedCollection;
    private List<ItemRow> itemRows;
    private const float VerticalRowOffset = 143.65f;
    
    [Header("Object References")]
    [SerializeField] private GameObject itemRowPrefab;
    [SerializeField] private Transform panel;

    private void Start()
    {
        itemRows = new List<ItemRow>();
    }

    public override void EnableComponent(bool instant = true)
    {
        base.EnableComponent(instant);

        CameraManager.ChangeCameraState(CursorLockMode.None);
        PlayerManager.DisableMovement();
    }
    
    public override void DisableComponent(bool instant = true)
    {
        base.DisableComponent(instant);

        CameraManager.ChangeCameraState(CursorLockMode.Locked);
        PlayerManager.EnableMovement();
    }
    
    public void SetUp()
    {
        if (playerInventory == null) playerInventory = PlayerManager
            .GetPlayer()
            .GetInventory();
        
        ShowCollection(ItemType.MeleeWeapon);
    }

    private void CreateRows()
    {
        itemRows ??= new List<ItemRow>();

        var items = playerInventory
            .GetCollection(currentDisplayedCollection)
            .GetItems();

        var rowCount = Math.Max(
            // ReSharper disable once PossibleLossOfFraction
            Mathf.CeilToInt((items.Count + 1) / 8), 3);

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            itemRows.Add(
                Instantiate(
                        itemRowPrefab, 
                        panel.transform.position + new Vector3(0f, -VerticalRowOffset * (rowIndex - 1) / 5.5f, 0f), 
                        Quaternion.identity, 
                        panel)
                    .GetComponent<ItemRow>());
            
            itemRows[rowIndex].SetUp(
                items.Count > rowIndex * 8
                    ? items.GetRange(rowIndex * 8,
                        items.Count % 8)
                    : new List<Item>());
        }
    }

    private void ClearRows()
    {
        itemRows ??= new List<ItemRow>();
        if (itemRows.Count == 0) return;
        for (var index = itemRows.Count - 1; index >= 0; index--)
        {
            Destroy(itemRows[index].gameObject);
        }
        itemRows.Clear();
    }

    private void ShowCollection(ItemType itemType)
    {
        currentDisplayedCollection = itemType;
        ClearRows();
        CreateRows();
    }
    
    public void ShowCollection(int itemType)
    {
        ShowCollection((ItemType)itemType);
    }
}
