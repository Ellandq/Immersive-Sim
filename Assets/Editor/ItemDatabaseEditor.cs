#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ItemDatabase database = (ItemDatabase)target;

        if (GUILayout.Button("Update Item List"))
        {
            UpdateItemList(database);
            EditorUtility.SetDirty(database);
        }
    }

    private void UpdateItemList(ItemDatabase database)
    {
        database.allItems = new List<ItemObject>(Resources.LoadAll<ItemObject>(""));
    }
}
#endif