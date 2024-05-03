using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Manager : MonoBehaviour, IManager
{
    private static UI_Manager Instance;
    [SerializeField] private UI_ItemList UI;

    private void Awake()
    {
        Instance = this;
    }

    public void SetUp()
    {
        foreach (var component in UI)
        {
            var uiComponent = component.Value;
            
            switch (component.Key)
            {
                case UI_Key.Reticle:
                    break;
                case UI_Key.StatDisplay_Health:
                    ((StatDisplay)uiComponent).SetUpDisplay(StatType.Health);
                    break;
                case UI_Key.StatDisplay_Stamina:
                    ((StatDisplay)uiComponent).SetUpDisplay(StatType.Stamina);
                    break;
                case UI_Key.StatDisplay_Mana:
                    ((StatDisplay)uiComponent).SetUpDisplay(StatType.Mana);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static UI_Manager GetInstance() { return Instance; }

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
    Reticle,
    // LEFT SECTION
    
    // RIGHT SECTION
    
    // TOP SECTION
    
    // BOTTOM SECTION
    StatDisplay_Health,
    StatDisplay_Stamina,
    StatDisplay_Mana
}


