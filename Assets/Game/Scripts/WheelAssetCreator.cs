using UnityEngine;
using UnityEditor;
using System.IO;

public class WheelAssetCreator
{
    [MenuItem("Tools/WheelOfFortune/Create All Assets")]
    public static void CreateAllAssets()
    {
        CreateFolders();
        CreateBronzeSlices();
        CreateSilverSlices();
        CreateGoldenSlices();
        CreateWheelConfigs();
        CreateZoneGroupConfigs();
        CreateZoneConfig();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All Wheel Assets Created Successfully!");
    }

    private static void CreateFolders()
    {
        string[] folders = {
            "Assets/_Game",
            "Assets/_Game/ScriptableObjects",
            "Assets/_Game/ScriptableObjects/Slices",
            "Assets/_Game/ScriptableObjects/Slices/Bronze",
            "Assets/_Game/ScriptableObjects/Slices/Silver",
            "Assets/_Game/ScriptableObjects/Slices/Golden",
            "Assets/_Game/ScriptableObjects/Wheels",
            "Assets/_Game/ScriptableObjects/Zones",
            "Assets/_Game/ScriptableObjects/Events"
        };

        foreach (var folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parent = Path.GetDirectoryName(folder).Replace("\\", "/");
                string newFolder = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parent, newFolder);
            }
        }

        AssetDatabase.Refresh();
    }

    private static void CreateBronzeSlices()
    {
        // Zone 1-9 Bronze Slices
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Gold100",
            "Gold x100", RewardType.Gold, 100, false, 30f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Cash500",
            "Cash x500", RewardType.Cash, 500, false, 25f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Chest",
            "Bronze Chest", RewardType.Chest, 1, false, 20f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Gold200",
            "Gold x200", RewardType.Gold, 200, false, 15f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Bomb",
            "Bomb", RewardType.Bomb, 0, true, 10f);

        // Zone 10-19 Bronze Slices
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Mid_Gold500",
            "Gold x500", RewardType.Gold, 500, false, 28f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Mid_Cash2000",
            "Cash x2000", RewardType.Cash, 2000, false, 22f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Mid_WeaponSkin",
            "Weapon Skin", RewardType.WeaponSkin, 1, false, 20f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Mid_Gold1000",
            "Gold x1000", RewardType.Gold, 1000, false, 15f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Mid_Bomb",
            "Bomb", RewardType.Bomb, 0, true, 15f);

        // Zone 20-29 Bronze Slices
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Late_Gold2000",
            "Gold x2000", RewardType.Gold, 2000, false, 25f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Late_Cash5000",
            "Cash x5000", RewardType.Cash, 5000, false, 22f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Late_WeaponSkin",
            "Rare Weapon", RewardType.WeaponSkin, 1, false, 18f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Late_Cosmetic",
            "Cosmetic", RewardType.Cosmetic, 1, false, 15f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Bronze/SliceData_Bronze_Late_Bomb",
            "Bomb", RewardType.Bomb, 0, true, 20f);
    }

    private static void CreateSilverSlices()
    {
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Silver/SliceData_Silver_Gold1000",
            "Gold x1000", RewardType.Gold, 1000, false, 30f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Silver/SliceData_Silver_Cash3000",
            "Cash x3000", RewardType.Cash, 3000, false, 25f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Silver/SliceData_Silver_Chest",
            "Silver Chest", RewardType.Chest, 1, false, 25f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Silver/SliceData_Silver_WeaponSkin",
            "Weapon Skin", RewardType.WeaponSkin, 1, false, 20f);
    }

    private static void CreateGoldenSlices()
    {
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Golden/SliceData_Golden_Gold10000",
            "Gold x10000", RewardType.Gold, 10000, false, 25f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Golden/SliceData_Golden_RareWeapon",
            "Rare Weapon", RewardType.WeaponSkin, 1, false, 20f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Golden/SliceData_Golden_Cosmetic",
            "Rare Cosmetic", RewardType.Cosmetic, 1, false, 20f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Golden/SliceData_Golden_Cash20000",
            "Cash x20000", RewardType.Cash, 20000, false, 20f);
        CreateSlice("Assets/_Game/ScriptableObjects/Slices/Golden/SliceData_Golden_Consumable",
            "Rare Consumable", RewardType.Consumable, 1, false, 15f);
    }

    private static void CreateWheelConfigs()
    {
        CreateWheelConfig("Assets/_Game/ScriptableObjects/Wheels/WheelConfig_Bronze_Early",
            WheelType.Bronze, 3f, 5f);
        CreateWheelConfig("Assets/_Game/ScriptableObjects/Wheels/WheelConfig_Bronze_Mid",
            WheelType.Bronze, 3f, 5f);
        CreateWheelConfig("Assets/_Game/ScriptableObjects/Wheels/WheelConfig_Bronze_Late",
            WheelType.Bronze, 3f, 5f);
        CreateWheelConfig("Assets/_Game/ScriptableObjects/Wheels/WheelConfig_Silver",
            WheelType.Silver, 3f, 5f);
        CreateWheelConfig("Assets/_Game/ScriptableObjects/Wheels/WheelConfig_Golden",
            WheelType.Golden, 4f, 6f);
    }

    private static void CreateZoneGroupConfigs()
    {
        CreateZoneGroup("Assets/_Game/ScriptableObjects/Zones/ZoneGroup_1_to_4", 1, 4);
        CreateZoneGroup("Assets/_Game/ScriptableObjects/Zones/ZoneGroup_6_to_9", 6, 9);
        CreateZoneGroup("Assets/_Game/ScriptableObjects/Zones/ZoneGroup_11_to_19", 11, 19);
        CreateZoneGroup("Assets/_Game/ScriptableObjects/Zones/ZoneGroup_21_to_29", 21, 29);
    }

    private static void CreateZoneConfig()
    {
        var zoneConfig = ScriptableObject.CreateInstance<ZoneConfigSO>();
        AssetDatabase.CreateAsset(zoneConfig,
            "Assets/_Game/ScriptableObjects/Zones/ZoneConfig.asset");
    }

    private static void CreateSlice(string path, string name, RewardType type,
        int amount, bool isBomb, float weight)
    {
        if (AssetDatabase.LoadAssetAtPath<SliceDataSO>(path + ".asset") != null)
            return;

        var slice = ScriptableObject.CreateInstance<SliceDataSO>();
        AssetDatabase.CreateAsset(slice, path + ".asset");
        EditorUtility.SetDirty(slice);
    }

    private static void CreateWheelConfig(string path, WheelType type,
        float minSpin, float maxSpin)
    {
        if (AssetDatabase.LoadAssetAtPath<WheelConfigSO>(path + ".asset") != null)
            return;

        var config = ScriptableObject.CreateInstance<WheelConfigSO>();
        AssetDatabase.CreateAsset(config, path + ".asset");
        EditorUtility.SetDirty(config);
    }

    private static void CreateZoneGroup(string path, int from, int to)
    {
        if (AssetDatabase.LoadAssetAtPath<ZoneGroupConfigSO>(path + ".asset") != null)
            return;

        var group = ScriptableObject.CreateInstance<ZoneGroupConfigSO>();
        AssetDatabase.CreateAsset(group, path + ".asset");
        EditorUtility.SetDirty(group);
    }
}