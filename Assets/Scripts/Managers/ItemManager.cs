using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[ExecuteInEditMode]
public class ItemManager : MonoBehaviour, IManager
{
    private static ItemManager Instance;
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("AssetBundles")] 
    private AssetBundle loadedBundle;
    private string currentLoadedBundlePath = "";
    private bool bundleLoaded;

    [Header("Special Bundles")] 
    private AssetBundle containerBundle;
    private AssetBundle ammunitionBundle;
    
    [ContextMenu("Set Instance")]
    private void SetInstance()
    {
        Instance = this;
    }

    public void SetUp()
    {
        containerBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "props/containers"));
    }

    public static ItemManager GetInstance()
    {
        if (Instance == null)
        {
            FindObjectOfType<ItemManager>().SetInstance();
        }

        return Instance;
    }

    #region Runtime

        private void UnloadBundle(string bundlePath = "")
        {
            if (bundlePath == currentLoadedBundlePath) return;
            currentLoadedBundlePath = "";
            loadedBundle.Unload(false);
            bundleLoaded = false;
        }

        private void LoadBundle(string bundlePath)
        {
            UnloadBundle(bundlePath);
            if (bundleLoaded) return;

            loadedBundle = AssetBundle.LoadFromFile(bundlePath);
            if (loadedBundle == null)
            {
                Debug.LogError("Failed to load asset bundle: " + bundlePath);
                bundleLoaded = false;
                currentLoadedBundlePath = "";
                return;
            }
            bundleLoaded = true;
            currentLoadedBundlePath = bundlePath;
            
        }
        
        public static GameObject GetContainerPrefab(ContainerType containerType)
        {
            return Instance.containerBundle.LoadAsset<GameObject>(containerType.ToString());
        }

        public static GameObject GetItemPrefab(ItemObject itemData)
        {
            Instance.LoadBundle(GetAssetBundlePath(itemData));
            return Instance.bundleLoaded ? Instance.loadedBundle.LoadAsset<GameObject>(itemData.HiddenName) : null;
        }

        private static string GetAssetBundlePath(ItemObject itemData)
        {
            return Path.Combine(Application.streamingAssetsPath, itemData.Collection);
        }

    #endregion

    #region Editor

        public static void GetItemPrefab(ItemObject itemData, Action<GameObject> callback)
        {
            var item = GetInstance().itemDatabase.GetByID(itemData.ID);
            var handle = Addressables.LoadAssetAsync<GameObject>(item.ToString());

            handle.Completed += (opHandle) =>
            {
                if (opHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(opHandle.Result);
                }
                else
                {
                    Debug.LogError("Failed to load item prefab: " + item.ToString());
                    callback?.Invoke(null);
                }
            };
        }

        public static void GetContainerPrefab(ContainerType containerType, Action<GameObject> callback)
        {
            var address = "props/containers/" + containerType.ToString().ToLower();
            var handle = Addressables.LoadAssetAsync<GameObject>(address);

            handle.Completed += (opHandle) =>
            {
                if (opHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(opHandle.Result);
                }
                else
                {
                    Debug.LogError("Failed to load container prefab: " + address);
                    callback?.Invoke(null);
                }
            };
        }

    #endregion

    public static ItemSection GetItemSection(ItemType itemType) { return (ItemSection)((int)itemType % 10); }

    public static List<ItemType> GetItemTypes(ItemSection itemSection)
    {
        return Enum.GetValues(typeof(ItemType)).Cast<ItemType>()
            .Where(itemType => (int)itemType % 10 == (int)itemSection).ToList();
    }
}

public enum ItemSection
{
    Weapon = 0,
    Armor = 1,
    Consumable = 2,
    Miscellaneous = 3
}

public enum ItemType 
{
    // Weapons
    MeleeWeapon = 0, RangedWeapon = 10, Staff = 20,
    // Ammunition
    Ammunition = 30,
    // Armor
    LightArmor = 1, MediumArmor = 11, HeavyArmor = 21, Robe = 31,
    // Consumables
    Potions = 2, Scrolls = 12, Runes = 22,
    // Misc
    Plant = 3, Ingredient = 13, Book = 23, Key = 33
}

public enum ContainerType
{
    Sack, 
    Chest
}
