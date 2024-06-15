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
    private static ItemDatabaseSettings settings;

    private const string ScriptableObjectPath = "Assets/ScriptableObjects/Items";
    private const string PrefabPath = "Assets/Prefabs/Items";
    
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
        itemDatabase = AssetDatabase.LoadAssetAtPath<ItemDatabase>(Path.Combine(ScriptableObjectPath, "ItemDatabase.asset"));
        settings = AssetDatabase.LoadAssetAtPath<ItemDatabaseSettings>(Path.Combine(ScriptableObjectPath, "ItemDatabaseSettings.asset"));
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

        // Use local variables to hold the tags and sections
        var tags = new Dictionary<string, string>(settings.Tags);
        var sections = new Dictionary<ItemSection, Dictionary<string, ItemType>>(settings.Sections);

        // Display tags
        EditorGUILayout.Space();
        tagsFoldout = EditorGUILayout.Foldout(tagsFoldout, "Tags", true);
        if (tagsFoldout)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var tag in tags.ToList())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tag.Key, GUILayout.Width(300));
                tags[tag.Key] = EditorGUILayout.TextField(tag.Value);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        // Display sections
        EditorGUILayout.Space();
        sectionsFoldout = EditorGUILayout.Foldout(sectionsFoldout, "Sections", true);
        if (sectionsFoldout)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var section in sections)
            {
                EditorGUILayout.LabelField(section.Key.ToString(), EditorStyles.boldLabel);

                EditorGUI.indentLevel++;

                foreach (var kvp in section.Value.ToList())
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.LabelField(kvp.Key, GUILayout.Width(280));
                    section.Value[kvp.Key] = (ItemType)EditorGUILayout.EnumPopup(kvp.Value);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        // Search bar
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        searchTerm = EditorGUILayout.TextField("Search", searchTerm);
        if (GUILayout.Button("Update Item List"))
        {
            RefreshItemDatabase();
        }
        EditorGUILayout.EndHorizontal();

        // Items display
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        var keysToModify = new List<string>();

        foreach (var item in itemDatabase.allItems.ToList())
        {
            if (string.IsNullOrEmpty(searchTerm) || item.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
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
                        SelectItemObject(item);
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
           
        }

        if (GUILayout.Button("Create New Item"))
        {
            CreateNewItem();
        }
        
        settings.Tags = tags;
        settings.Sections = sections;

        EditorUtility.SetDirty(settings);
    }
    
    private void PopulateTags()
    {
        var tags = settings.Tags;
        PopulateTagsRecursive(tags, ScriptableObjectPath);
        settings.Tags = tags;
    }
    
    private void PopulateTagsRecursive(Dictionary<string, string> tags, string folderPath)
    {
        var subfolders = AssetDatabase.GetSubFolders(folderPath);
        foreach (var subfolder in subfolders)
        {
            var folderName = new DirectoryInfo(subfolder).Name;
            var key = GetFolderPathRelativeToScriptableObjects(subfolder);
            if (!tags.ContainsKey(key))
            {
                tags.Add(key, folderName);
            }
            PopulateTagsRecursive(tags, subfolder);
        }
    }

    private string GetFolderPathRelativeToScriptableObjects(string fullPath)
    {
        return fullPath.Substring(ScriptableObjectPath.Length + 1);
    }

    private void PopulateSections()
    {
        var sections = settings.Sections;
        var enumValues = Enum.GetValues(typeof(ItemSection));
        
        foreach (ItemSection section in enumValues)
        {
            if (!sections.ContainsKey(section))
            {
                sections[section] = new Dictionary<string, ItemType>();
            }

            var folderPath = Path.Combine(ScriptableObjectPath, section.ToString());
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(ScriptableObjectPath, section.ToString());
            }
            PopulateSubsections(sections, folderPath, section);
        }

        settings.Sections = sections;
    }

    private void PopulateSubsections(Dictionary<ItemSection, Dictionary<string, ItemType>> sections, string path, ItemSection currentSection)
    {
        var subfolders = AssetDatabase.GetSubFolders(path);
        
        foreach (var subfolderPath in subfolders)
        {
            var folderName = new DirectoryInfo(subfolderPath).Name;
            if (!sections[currentSection].ContainsKey(folderName))
            {
                sections[currentSection].Add(folderName, 0);
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
        if (!AssetDatabase.IsValidFolder(PrefabPath))
        {
            Debug.LogError($"The prefab folder path is not valid: '{PrefabPath}'");
            return;
        }

        if (!AssetDatabase.IsValidFolder(ScriptableObjectPath))
        {
            Debug.LogError($"The scriptable object folder path is not valid: '{ScriptableObjectPath}'");
            return;
        }

        var database = new List<ItemDatabaseEntity>();

        CheckFolderForPrefabs(database, PrefabPath);
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
            var relativePath = assetPath.Substring(PrefabPath.Length + 1);
            var directoryName = Path.GetDirectoryName(relativePath);
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            
            if (!databaseEntries.Any(entry => entry.Name == fileName && entry.Path == directoryName))
            {
                databaseEntries.Add(new ItemDatabaseEntity(fileName, directoryName));
            }
            
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                var currentAssetBundle = importer.assetBundleName;
                
                if (!string.IsNullOrEmpty(currentAssetBundle)) continue;
                importer.assetBundleName = directoryName.Substring(1);
                importer.SaveAndReimport();
            }
            else
            {
                Debug.LogWarning($"Failed to get AssetImporter for '{assetPath}'");
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
                var itemPath = ScriptableObjectPath + '/' + Path.Combine(databaseEntries[i].Path, databaseEntries[i].Name + ".asset");
                var itemObject = AssetDatabase.LoadAssetAtPath<ItemObject>(itemPath);

                databaseEntries[i].ItemObject = itemObject == null ? CreateItemObject(databaseEntries[i], itemPath) : itemObject;
                itemPath = ScriptableObjectPath + '/' + Path.Combine(databaseEntries[i].Path, databaseEntries[i].Name + "_Modifier.asset");
                var itemModifier = AssetDatabase.LoadAssetAtPath<ItemModifier>(itemPath);

                if (itemModifier != null)
                {
                    databaseEntries[i].ItemModifier = itemModifier;
                }
            }
        }
    }

    private static ItemObject CreateItemObject(ItemDatabaseEntity referenceObject, string path)
    {
        var itemObject = CreateInstance<ItemObject>();
        
        var directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh(); 
        }

        var trimmedPath = directoryPath.Substring(ScriptableObjectPath.Length + 1);
    
        itemObject.HiddenName = referenceObject.Name;
        itemObject.Collection = settings[trimmedPath];
        itemObject.ItemType = settings.GetItemType(trimmedPath);
    
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
        AssetDatabase.CreateAsset(itemDatabase, Path.Combine(ScriptableObjectPath, "ItemDatabase.asset"));
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = itemDatabase;
    }

    private void CreateNewItem()
    {
        var path = Path.Combine(ScriptableObjectPath, "NewItem.asset");
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
    
    private void SelectItemObject(ItemDatabaseEntity item)
    {
        var itemObject = item.ItemObject;
        if (itemObject != null)
        {
            // Focus the Project window on the item object
            EditorGUIUtility.PingObject(itemObject);

            // Select the item object in the Project window
            Selection.activeObject = itemObject;

            // Ensure the Inspector window is visible and focused
            EditorApplication.delayCall += () =>
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(itemObject);
                Selection.activeObject = itemObject;
            };
        }
        else
        {
            Debug.LogWarning("Item Object is null. Cannot select.");
        }
    }


}
#endif
