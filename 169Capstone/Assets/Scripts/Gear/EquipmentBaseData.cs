using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Item Enums

public enum InventoryItemSlot
{
    Weapon,
    Helmet,
    Accessory,
    Legs,  // Legwear?

    enumSize
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,

    enumSize    // Also "none"
}

// Every item has its own unique ID (name)
public enum ItemID
{
    // Weapons
    BerserkersZweihander,
    BowAndArrows,
    NanoKnuckles,
    Rapier,
    RayGun,

    // Helmets
    HelmOfTheRam,
    HoloGlasses,

    // Accessories
    WristRocket,
    RingOfSnowstorms,
    MurphysClaw,
    QuantumKunai,

    // Boots
    WingedBoots,
    QuantumLeggings,
    PropulsionHeels,
    TrousersOfFortitude,

    // Default
    enumSize
}

#endregion

[CreateAssetMenu(menuName = "Gear/EquipmentData")]
public class EquipmentBaseData : ScriptableObject
{
    [Tooltip("Player facing item name")]
    [SerializeField] private string itemName;
    
    [Tooltip("Internal ID")]
    [SerializeField] private ItemID itemID;

    [Tooltip("IF equipping this item, unlocks a journal content entry (or multiple), put that here; if not, leave it empty")]
    [SerializeField] private JournalContentID[] journalEntriesUnlocked;

    [Tooltip("Just the model, for dropping on the ground")]
    [SerializeField] private GameObject itemDropModelPrefab;

    [Tooltip("Actual prefab of the item (no model) created once EQUIPPED")]
    [SerializeField] private GameObject equippedItemPrefab;

    [Tooltip("Just the model, sized and angled for attaching to the player. WEAPONS ONLY!")]
    [SerializeField] private GameObject equippedWeaponModelPrefab;

    [SerializeField] private InventoryItemSlot itemSlot;
    [SerializeField] private int baseCost = 10;

    [TextArea(4,20)]
    [SerializeField] private string shortDescription;
    [TextArea(15,20)]
    [SerializeField] private string longDescription;

    [SerializeField] private Sprite icon;

    [SerializeField] private PlayerStatName primaryStat;

    [SerializeField] private StatType primaryItemLine;


    public string ItemName()
    {
        return itemName;
    }

    public ItemID ItemID()
    {
        return itemID;
    }

    public GameObject ItemDropModelPrefab()
    {
        return itemDropModelPrefab;
    }

    public GameObject EquippedWeaponModelPrefab()
    {
        if(itemSlot != InventoryItemSlot.Weapon){
            Debug.LogError("Cannot retrieve weapon model prefab for item type: " + itemSlot);
            return null;
        }
        return equippedWeaponModelPrefab;
    }

    public GameObject EquippedItemPrefab()
    {
        return equippedItemPrefab;
    }

    public InventoryItemSlot ItemSlot()
    {
        return itemSlot;
    }

    public int BaseCost()
    {
        return baseCost;
    }

    public string ShortDescription()
    {
        return shortDescription;
    }

    public string LongDescription()
    {
        return longDescription;
    }

    public Sprite Icon()
    {
        return icon;
    }

    public PlayerStatName PrimaryStat()
    {
        return primaryStat;
    }

    public StatType PrimaryItemLine()
    {
        return primaryItemLine;
    }

    public JournalContentID[] JournalEntriesUnlocked()
    {
        return journalEntriesUnlocked;
    }
}
