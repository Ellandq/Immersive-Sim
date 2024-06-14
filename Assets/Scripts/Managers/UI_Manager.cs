using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Manager : MonoBehaviour, IManager
{
    private static UI_Manager Instance;
    
    [Header ("UI References")]
    [SerializeField] private UI_ItemList UI;

    [Header("UI Info")] 
    private bool isPauseMenuEnabled;

    private void Awake()
    {
        Instance = this;
        isPauseMenuEnabled = false;
    }

    private void Start()
    {
        var inputHandle = InputManager.GetInputHandle();
        
        inputHandle.AddListenerOnInputAction(HandleInventoryDisplay, "Inventory");
    }
    
    public void SetUp()
    {
        foreach (var component in UI)
        {
            var uiComponent = component.Value;
            
            switch (component.Key)
            {
                case UI_Key.Reticle:
                    component.IsEnabled = true;
                    break;
                case UI_Key.StatDisplay_Health:
                    ((UI_StatDisplay)uiComponent).SetUpDisplay(StatType.Health);
                    component.IsEnabled = true;
                    break;
                case UI_Key.StatDisplay_Stamina:
                    ((UI_StatDisplay)uiComponent).SetUpDisplay(StatType.Stamina);
                    component.IsEnabled = true;
                    break;
                case UI_Key.StatDisplay_Mana:
                    ((UI_StatDisplay)uiComponent).SetUpDisplay(StatType.Mana);
                    component.IsEnabled = true;
                    break;
                case UI_Key.Inventory:
                    component.IsEnabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static UI_Manager GetInstance() { return Instance; }

    private void HandleInventoryDisplay(ButtonState state)
    {
        if (state == ButtonState.Up || isPauseMenuEnabled) return;
        
        var display = (UI_Inventory)UI.GetValue(UI_Key.Inventory);

        if (display.IsEnabled)
        {
            UI.GetValue(UI_Key.Reticle).EnableComponent();
            display.DisableComponent();
            CameraManager.ChangeCameraState(CursorLockMode.Locked);
            PlayerManager.EnableMovement();
            PlayerManager.EnableInteractions();
        }
        else
        {
            UI.GetValue(UI_Key.Reticle).DisableComponent();
            display.EnableComponent();
            display.SetUp();
            CameraManager.ChangeCameraState(CursorLockMode.None);
            PlayerManager.DisableMovement();
            PlayerManager.DisableInteractions();
        }
    }

}

[Serializable]
public class UI_ItemList : IEnumerable<UI_Item>
{
    [SerializeField] private List<UI_Item> items = new List<UI_Item>();

    public void Add(UI_Key key, UI_Component value)
    {
        if (ContainsKey(key))
        {
            Debug.LogWarning("Key already exists: " + key);
            return;
        }

        items.Add(new UI_Item(key, value));
    }

    public bool Remove(UI_Key key)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i].Key != key) continue;
            items.RemoveAt(i);
            return true;
        }
        return false;
    }

    public bool ContainsKey(UI_Key key)
    {
        return items.Any(item => item.Key == key);
    }

    public UI_Component GetValue(UI_Key key)
    {
        return (from item in items where item.Key == key select item.Value).FirstOrDefault();
    }
    
    public IEnumerator<UI_Item> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Serializable]
public class UI_Item
{
    [SerializeField] public UI_Key Key;
    [SerializeField] public UI_Component Value;
    
    public bool IsEnabled { get; set; }

    public UI_Item(UI_Key key, UI_Component value)
    {
        Key = key;
        Value = value;
    }
}

public enum UI_Key
{
    // HUD
    // MIDDLE SECTION
    Reticle, Inventory,
    // LEFT SECTION
    
    // RIGHT SECTION
    
    // TOP SECTION
    
    // BOTTOM SECTION
    StatDisplay_Health,
    StatDisplay_Stamina,
    StatDisplay_Mana
}


