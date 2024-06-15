#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ItemDatabaseEditorWindow : EditorWindow
{
    private ItemDatabase itemDatabase;

    private const string scriptableObjectPath = "Assets/ScriptableObjects/Items";
    private const string prefabPath = "Assets/Prefabs/Items";
    
    private string searchTerm = "";
    private Vector2 scrollPos;

    [MenuItem("Tools/Item Database")]
    public static void ShowWindow()
    {
        GetWindow<ItemDatabaseEditorWindow>("Item Database");
    }

    private void OnEnable()
    {
        itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>(Path.Combine(scriptableObjectPath, "ItemDatabase.asset"));
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase.asset not found.");
        }
    }

    private void OnGUI()
    {
        if (itemDatabase == null)
        {
            EditorGUILayout.HelpBox("Item Database not found. Please create an Item Database.", MessageType.Error);
            if (GUILayout.Button("Create Item Database"))
            {
                CreateItemDatabase();
            }
            return;
        }

        EditorGUILayout.BeginHorizontal();
        searchTerm = EditorGUILayout.TextField("Search", searchTerm);

        if (GUILayout.Button("Update Item List"))
        {
            RefreshItemDatabase();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var item in itemDatabase.allItems)
        {
            if (string.IsNullOrEmpty(searchTerm) || item.Name.Contains(searchTerm))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item.Name);

                if (GUILayout.Button("Edit"))
                {
                    Selection.activeObject = item.ItemObject;
                }

                if (GUILayout.Button("Delete"))
                {
                    DeleteItem(item);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Create New Item"))
        {
            CreateNewItem();
        }
    }

    private void RefreshItemDatabase()
    {
        ClearDatabase();
        PopulateDatabase();
        EditorUtility.SetDirty(itemDatabase);
        AssetDatabase.SaveAssets();
    }

    private void PopulateDatabase()
    {
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            Debug.LogError($"The prefab folder path is not valid: '{prefabPath}'");
            return;
        }

        if (!AssetDatabase.IsValidFolder(scriptableObjectPath))
        {
            Debug.LogError($"The scriptable object folder path is not valid: '{scriptableObjectPath}'");
            return;
        }

        var database = new List<ItemDatabaseEntity>();

        CheckFolderForPrefabs(database, prefabPath);
        CheckFolderForScriptableObjects(database);

        itemDatabase.allItems = database;
    }

    private static void CheckFolderForPrefabs(List<ItemDatabaseEntity> databaseEntries, string path)
    {
        var subfolders = AssetDatabase.GetSubFolders(path);
        foreach (var subfolderPath in subfolders)
        {
            CheckFolderForPrefabs(databaseEntries, subfolderPath);
        }

        var assetGuids = AssetDatabase.FindAssets("", new[] { path });

        foreach (var guid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (assetPath.EndsWith(".prefab"))
            {
                var relativePath = assetPath.Substring(prefabPath.Length);
                var directoryName = Path.GetDirectoryName(relativePath);
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                
                if (!databaseEntries.Any(entry => entry.Name == fileName && entry.Path == directoryName))
                {
                    databaseEntries.Add(new ItemDatabaseEntity(fileName, directoryName));
                }
            }
        }
    }

    private static void CheckFolderForScriptableObjects(List<ItemDatabaseEntity> databaseEntries)
    {
        var currentIndex = 0;
        while (currentIndex < databaseEntries.Count)
        {
            var currentPath = databaseEntries[currentIndex].Path;
            var startingIndex = currentIndex;
            for (currentIndex++; currentIndex < databaseEntries.Count; currentIndex++)
            {
                if (currentPath != databaseEntries[currentIndex].Path) break;
            }

            for (var i = startingIndex; i < currentIndex; i++)
            {
                var itemPath = scriptableObjectPath + Path.Combine(databaseEntries[i].Path, databaseEntries[i].Name + ".asset");
                var itemObject = AssetDatabase.LoadAssetAtPath<ItemObject>(itemPath);

                if (itemObject == null)
                {
                    CreateItemObject(itemPath);
                }
                else
                {
                    databaseEntries[i].ItemObject = itemObject;
                }
                itemPath = scriptableObjectPath + Path.Combine(databaseEntries[i].Path, databaseEntries[i].Name + "_Modifier.asset");
                var itemModifier = AssetDatabase.LoadAssetAtPath<ItemModifier>(itemPath);

                if (itemModifier != null)
                {
                    databaseEntries[i].ItemModifier = itemModifier;
                }
            }
        }
    }

    private static void CreateItemObject(string path)
    {
        var itemObject = CreateInstance<ItemObject>();
        AssetDatabase.CreateAsset(itemObject, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }

    private void ClearDatabase()
    {
        itemDatabase.allItems.Clear();
    }

    private void CreateItemDatabase()
    {
        itemDatabase = CreateInstance<ItemDatabase>();
        AssetDatabase.CreateAsset(itemDatabase, Path.Combine(scriptableObjectPath, "ItemDatabase.asset"));
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = itemDatabase;
    }

    private void CreateNewItem()
    {
        var path = Path.Combine(scriptableObjectPath, "NewItem.asset");
        var itemObject = CreateInstance<ItemObject>();
        AssetDatabase.CreateAsset(itemObject, path);
        AssetDatabase.SaveAssets();
        itemDatabase.allItems.Add(new ItemDatabaseEntity("NewItem", "NewItem"));
        EditorUtility.SetDirty(itemDatabase);
        AssetDatabase.SaveAssets();
    }

    private void DeleteItem(ItemDatabaseEntity item)
    {
        if (item.ItemObject != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.ItemObject));
        }
        if (item.ItemModifier != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.ItemModifier));
        }
        itemDatabase.allItems.Remove(item);
        EditorUtility.SetDirty(itemDatabase);
        AssetDatabase.SaveAssets();
    }
}
#endif
