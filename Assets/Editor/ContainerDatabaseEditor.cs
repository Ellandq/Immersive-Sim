using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

[CustomEditor(typeof(ContainerDatabase))]
public class ContainerDatabaseEditor : Editor
{
    private const string BasePath = @"Assets\Prefabs\Containers";
    
    private string containerName = "";
    private ContainerType containerType;
    private ContainerDatabaseEntity randomContainer;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var database = (ContainerDatabase)target;
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Get Container By Name", EditorStyles.boldLabel);
        containerName = EditorGUILayout.TextField("Container Name", containerName);

        if (GUILayout.Button("Get By Name"))
        {
            ContainerDatabaseEntity container = database.GetByName(containerName);
            if (container != null)
            {
                Debug.Log("Found Container: " + container);
            }
            else
            {
                Debug.LogWarning("Container not found!");
            }
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Get Random Container By Type", EditorStyles.boldLabel);
        containerType = (ContainerType)EditorGUILayout.EnumPopup("Container Type", containerType);

        if (GUILayout.Button("Get Random Container"))
        {
            randomContainer = database.GetRandomContainer(containerType);
            if (randomContainer != null)
            {
                Debug.Log("Random Container: " + randomContainer);
            }
            else
            {
                Debug.LogWarning("No containers of this type found!");
            }
        }
        
        EditorGUILayout.Space();
        
        if (randomContainer != null)
        {
            EditorGUILayout.LabelField("Random Container", randomContainer.ToString());
        }

        // Add space
        EditorGUILayout.Space();

        // Add button to refresh the database
        if (GUILayout.Button("Refresh Database"))
        {
            RefreshDatabase();
        }
    }
    
    public void RefreshDatabase()
    {
        var database = (ContainerDatabase)target;
        var folderPaths = AssetDatabase.GetSubFolders(BasePath);
        var folders = folderPaths.Select(path => new DirectoryInfo(path).Name).ToList();

        foreach (ContainerType type in Enum.GetValues(typeof(ContainerType)))
        {
            
            if (!folders.Contains(type.ToString()))
            {
                AssetDatabase.CreateFolder(BasePath, type.ToString());
                folderPaths = AssetDatabase.GetSubFolders(BasePath);
            }
            var path = folderPaths.FirstOrDefault(fPath => fPath.EndsWith(type.ToString()));
            UpdateItemsFromDirectory(database, path, type);
        }
    }

    public void UpdateItemsFromDirectory(ContainerDatabase database, string path, ContainerType type)
    {
        
        var assetGuids = AssetDatabase.FindAssets("", new[] { path });

        foreach (var guid in assetGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!assetPath.EndsWith(".prefab")) continue;
            var assetName = Path.GetFileNameWithoutExtension(assetPath);

            if (!database.allContainers.Any(entry => entry.Name == assetName))
            {
                database.allContainers.Add(new ContainerDatabaseEntity(type, assetName));
            }
            
            var entity = database.GetByName(assetName);
            ManageAddressables(guid, entity.ToString());

            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                var currentAssetBundle = importer.assetBundleName;
                    
                if (!string.IsNullOrEmpty(currentAssetBundle)) continue;
                importer.assetBundleName = entity.GetAssetBundleName();
                importer.SaveAndReimport();
            }
            else
            {
                Debug.LogWarning($"Failed to get AssetImporter for '{assetPath}'");
            }
        }
    }
    
    private static void ManageAddressables(string guid, string newAddress)
    {
        AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
            guid, AddressableAssetSettingsDefaultObject.Settings.DefaultGroup, false, true);
        AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid).address = newAddress;
    }
}
