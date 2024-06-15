#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ItemDatabaseEditorWindow : EditorWindow
{
    private ItemDatabase itemDatabase;
    private ItemDatabaseSettings settings;

    [SerializeField]
    private Dictionary<string, string> Tags;
    [SerializeField]
    private Dictionary<ItemSection, Dictionary<string, ItemType>> Sections;

    private const string scriptableObjectPath = "Assets/ScriptableObjects/Items";
    private const string prefabPath = "Assets/Prefabs/Items";
    
    private string searchTerm = "";
    private Vector2 scrollPos;
    private Dictionary<string, bool> itemFoldouts = new Dictionary<string, bool>();

    private bool tagsFoldout = false;
    private bool sectionsFoldout = false;

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
        
        PopulateTags();
        PopulateSections();
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

        tagsFoldout = EditorGUILayout.Foldout(tagsFoldout, "Tags", true);
        if (tagsFoldout)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var tag in Tags.ToList())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tag.Key, GUILayout.Width(300));
                Tags[tag.Key] = EditorGUILayout.TextField(tag.Value);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        sectionsFoldout = EditorGUILayout.Foldout(sectionsFoldout, "Sections", true);
        if (sectionsFoldout)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var section in Sections)
            {
                EditorGUILayout.LabelField(section.Key.ToString(), EditorStyles.boldLabel);

                EditorGUI.indentLevel++; // Increase indentation for subsections

                foreach (var kvp in section.Value.ToList())
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20); // Adjust the indentation here as needed
                    EditorGUILayout.LabelField(kvp.Key, GUILayout.Width(280));
                    section.Value[kvp.Key] = (ItemType)EditorGUILayout.EnumPopup(kvp.Value);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--; // Restore indentation level
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        var keysToModify = new List<string>();

        foreach (var item in itemDatabase.allItems)
        {
            if (string.IsNullOrEmpty(searchTerm) || item.Name.Contains(searchTerm))
            {
                if (!itemFoldouts.ContainsKey(item.Name))
                {
                    itemFoldouts[item.Name] = false;
                }

                itemFoldouts[item.Name] = EditorGUILayout.Foldout(itemFoldouts[item.Name], item.Name);

                if (itemFoldouts[item.Name])
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name:", GUILayout.Width(50));
                    EditorGUILayout.LabelField(item.Name);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Path:", GUILayout.Width(50));
                    EditorGUILayout.LabelField(item.Path);
                    EditorGUILayout.EndHorizontal();

                    if (item.ItemObject != null)
                    {
                        EditorGUILayout.ObjectField("Item Object:", item.ItemObject, typeof(ItemObject), false);
                    }

                    if (item.ItemModifier != null)
                    {
                        EditorGUILayout.ObjectField("Item Modifier:", item.ItemModifier, typeof(ItemModifier), false);
                    }

                    EditorGUILayout.BeginHorizontal();
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

            keysToModify.Add(item.Name);
        }
        EditorGUILayout.EndScrollView();

        foreach (var key in keysToModify)
        {
            // Modify itemFoldouts dictionary based on some condition if needed
        }

        if (GUILayout.Button("Create New Item"))
        {
            CreateNewItem();
        }
    }

    
    private void PopulateTags()
    {
        Tags ??= new Dictionary<string, string>();
        PopulateTagsRecursive(scriptableObjectPath);
    }

    private void PopulateTagsRecursive(string folderPath)
    {
        var subfolders = AssetDatabase.GetSubFolders(folderPath);
        foreach (var subfolder in subfolders)
        {
            var folderName = new DirectoryInfo(subfolder).Name;
            var key = GetFolderPathRelativeToScriptableObjects(subfolder);
            if (!Tags.ContainsKey(key))
            {
                Tags[key] = folderName;
            }
            PopulateTagsRecursive(subfolder);
        }
    }

    private string GetFolderPathRelativeToScriptableObjects(string fullPath)
    {
        return fullPath.Substring(scriptableObjectPath.Length + 1);
    }

    private void PopulateSections()
    {
        if (Sections == null)
        {
            Sections = new Dictionary<ItemSection, Dictionary<string, ItemType>>();
        }

        var enumValues = Enum.GetValues(typeof(ItemSection));
        foreach (ItemSection section in enumValues)
        {
            if (!Sections.ContainsKey(section))
            {
                Sections[section] = new Dictionary<string, ItemType>();
            }

            var folderPath = Path.Combine(scriptableObjectPath, section.ToString());
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(scriptableObjectPath, section.ToString());
            }
            PopulateSubsections(folderPath, section);
        }
    }

    private void PopulateSubsections(string path, ItemSection currentSection)
    {
        var subfolders = AssetDatabase.GetSubFolders(path);
        
        foreach (var subfolderPath in subfolders)
        {
            var folderName = new DirectoryInfo(subfolderPath).Name;
            if (!Sections[currentSection].ContainsKey(folderName))
            {
                Sections[currentSection].Add(folderName, 0);  // Add only if key does not exist
            }
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
            if (!assetPath.EndsWith(".prefab")) continue;
            var relativePath = assetPath.Substring(prefabPath.Length);
            var directoryName = Path.GetDirectoryName(relativePath);
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
                
            if (!databaseEntries.Any(entry => entry.Name == fileName && entry.Path == directoryName))
            {
                databaseEntries.Add(new ItemDatabaseEntity(fileName, directoryName));
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

                databaseEntries[i].ItemObject = itemObject == null ? CreateItemObject(itemPath) : itemObject;
                itemPath = scriptableObjectPath + Path.Combine(databaseEntries[i].Path, databaseEntries[i].Name + "_Modifier.asset");
                var itemModifier = AssetDatabase.LoadAssetAtPath<ItemModifier>(itemPath);

                if (itemModifier != null)
                {
                    databaseEntries[i].ItemModifier = itemModifier;
                }
            }
        }
    }

    private static ItemObject CreateItemObject(string path)
    {
        var itemObject = CreateInstance<ItemObject>();
        AssetDatabase.CreateAsset(itemObject, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        return itemObject;
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
