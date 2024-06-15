using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabaseSettings", menuName = "Item Database Settings")]
public class ItemDatabaseSettings : ScriptableObject
{
    public Dictionary<string, string> Tags;
    public Dictionary<ItemSection, Dictionary<string, ItemType>> Sections;
}