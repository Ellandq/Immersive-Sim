using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabaseSettings", menuName = "Item Database Settings")]
public class ItemDatabaseSettings : ScriptableObject
{
    [SerializeField]
    private List<ItemDatabaseSettings_Tags> tags;
    [SerializeField]
    private List<ItemDatabaseSettings_Section> sections;


    public Dictionary<string, string> Tags
    {
        get
        {
            return tags.ToDictionary(tag => tag.Key, tag => tag.Value);
        }
        set
        {
            tags.Clear();
            foreach (var kvp in value)
            {
                tags.Add(new ItemDatabaseSettings_Tags(kvp));
            }
        }
    }
    
    public string this[string key]{
        get
        {
            return tags.FirstOrDefault(tag => tag.Key == key)?.Value;
        }
    }
    
    public Dictionary<string, ItemType> this[ItemSection key]{
        get
        {
            return sections.FirstOrDefault(section => section.Section == key)?.Subsections
                .ToDictionary(subsection => subsection.Path, subsection => subsection.ItemType);
        }
    }
    
    public Dictionary<ItemSection, Dictionary<string, ItemType>> Sections
    {
        get
        {
            return sections.ToDictionary(section => section.Section, section => section.Subsections.ToDictionary(
                subsection => subsection.Path, subsection => subsection.ItemType));
        }
        set
        {
            sections.Clear();
            foreach (var kvp in value)
            {
                sections.Add(new ItemDatabaseSettings_Section(kvp));
            }
        }
    }
}

[Serializable]
public class ItemDatabaseSettings_Tags
{
    public string Key;
    public string Value;

    public ItemDatabaseSettings_Tags(KeyValuePair<string, string> keyValuePair)
    {
        Key = keyValuePair.Key;
        Value = keyValuePair.Value;
    }
}

[Serializable]
public class ItemDatabaseSettings_Section
{
    public ItemSection Section;
    public List<ItemDatabaseSettings_Subsection> Subsections;
    
    public ItemDatabaseSettings_Section(KeyValuePair<ItemSection, Dictionary<string, ItemType>> keyValuePair)
    {
        Section = keyValuePair.Key;
        Subsections = keyValuePair.Value.Select(kv => new ItemDatabaseSettings_Subsection(kv.Key, kv.Value)).ToList();
    }
}

[Serializable]
public class ItemDatabaseSettings_Subsection
{
    public string Path;
    public ItemType ItemType;

    public ItemDatabaseSettings_Subsection(string path, ItemType itemType)
    {
        Path = path;
        ItemType = itemType;
    }
}
