using System.Runtime.CompilerServices;
using BepInEx;
using UnityEngine;
using ItemManager;
using HarmonyLib;

namespace GimmeDatGreatsword;

[BepInPlugin("GimmeDatGreatSword", "GimmeDatGreatSword", "1.0.0")]
public class Main : BaseUnityPlugin
{
    public static bool cloningDone = false;
    void Awake()
    {
        Harmony harmony = new("GimmeDatGreatSword");
        harmony.Patch(AccessTools.DeclaredMethod(typeof(FejdStartup), nameof(FejdStartup.Awake)), prefix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(Main), nameof(CloneItems)), Priority.VeryHigh));
        RuntimeHelpers.RunClassConstructor(typeof(PrefabManager).TypeHandle);
    }

    public static void CloneItems(FejdStartup __instance)
    {
        if (Main.cloningDone) return;
        Main.cloningDone = true;

        // create clone container
        var containerObj = new GameObject();
        containerObj.SetActive(false);
        DontDestroyOnLoad(containerObj);
        var container = containerObj.transform;

        // get base greatsword prefab
        var basePrefab = __instance.m_objectDBPrefab.GetComponent<ObjectDB>().m_items.First(i => i.name.Equals("THSwordKrom", StringComparison.Ordinal));

        // bronze
        Item bronze = new(CreateClone("GS_bronze"), false);
        bronze.Name.English("Bronze Greatsword");
        bronze.Description.English("Greatsword made from a large amount of bronze");
        bronze.SetMetalColor("#CD7F32");
        bronze.SetDamage(55, 10);
        bronze.Snapshot();

        bronze.Crafting.Add(CraftingTable.Forge, 0);
        bronze.RequiredItems.Add("Bronze", 12);
        bronze.RequiredItems.Add("Wood", 5);
        bronze.RequiredItems.Add("LeatherScraps", 5);

        bronze.RequiredUpgradeItems.Add("Bronze", 6);

        bronze.Configurable = Configurability.Full;

        // iron
        Item iron = new(CreateClone("GS_iron"), false);
        iron.Name.English("Iron Greatsword");
        iron.Description.English("Greatsword made from a large amount of iron");
        iron.SetMetalColor("#aecee4");
        iron.SetDamage(75, 10);
        iron.Snapshot();

        iron.Crafting.Add(CraftingTable.Forge, 0);
        iron.RequiredItems.Add("Iron", 30);
        iron.RequiredItems.Add("Wood", 5);
        iron.RequiredItems.Add("LeatherScraps", 5);

        iron.RequiredUpgradeItems.Add("Iron", 15);

        iron.Configurable = Configurability.Full;

        // silver
        Item silver = new(CreateClone("GS_silver"), false);
        silver.Name.English("Silver Greatsword");
        silver.Description.English("Greatsword made from a large amount of silver");
        silver.SetMetalColor("#ffffff");
        silver.SetDamage(115, 10);
        silver.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.m_spirit = 45;
        silver.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_damagesPerLevel.m_spirit = 7;
        silver.Snapshot();

        silver.Crafting.Add(CraftingTable.Forge, 0);
        silver.RequiredItems.Add("Silver", 60);
        silver.RequiredItems.Add("Iron", 5);
        silver.RequiredItems.Add("Wood", 5);
        silver.RequiredItems.Add("LeatherScraps", 5);

        silver.RequiredUpgradeItems.Add("Silver", 30);
        silver.RequiredUpgradeItems.Add("Iron", 3);

        silver.Configurable = Configurability.Full;

        // blackmetal
        Item blackmetal = new(CreateClone("GS_blackmetal"), false);
        blackmetal.Name.English("Blackmetal Greatsword");
        blackmetal.Description.English("Greatsword made from a large amount of blackmetal");
        blackmetal.SetDiffuseColor("#3d3d3d");
        blackmetal.SetMetalColor("#00d31e");
        blackmetal.SetDamage(150, 10);
        blackmetal.Snapshot();

        blackmetal.Crafting.Add(CraftingTable.Forge, 0);
        blackmetal.RequiredItems.Add("BlackMetal", 30);
        blackmetal.RequiredItems.Add("LinenThread", 5);
        blackmetal.RequiredItems.Add("LeatherScraps", 5);
        blackmetal.RequiredItems.Add("FineWood", 5);

        blackmetal.RequiredUpgradeItems.Add("BlackMetal", 15);
        blackmetal.RequiredUpgradeItems.Add("LinenThread", 2);

        blackmetal.Configurable = Configurability.Full;

        GameObject CreateClone(string name)
        {
            var clone = Instantiate(basePrefab, container);
            clone.name = name;
            clone.GetComponent<ItemDrop>().m_itemData.m_shared.m_name = $"${name}";
            clone.GetComponent<ItemDrop>().m_itemData.m_shared.m_description = $"${name}_description";
            return clone;
        }
    }
}

public static class HelperShit
{
    public static void SetMetalColor(this Item item, string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color))
        {
            item.Prefab.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(r => r.material.SetColor("_MetalColor", color));
        }
    }

    public static void SetDiffuseColor(this Item item, string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color))
        {
            item.Prefab.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(r => r.material.SetColor("_Color", color));
        }
    }

    public static void SetDamage(this Item item, int damage, int perLevel)
    {
        var drop = item.Prefab.GetComponent<ItemDrop>();
        drop.m_itemData.m_shared.m_damages.m_slash = damage;
        drop.m_itemData.m_shared.m_damagesPerLevel.m_slash = damage;
    }
}