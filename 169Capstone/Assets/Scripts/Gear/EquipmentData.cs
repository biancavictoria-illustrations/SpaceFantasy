using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: set these to whatever we're actually calling them in game (player-facing names)
public enum InventoryItemSlot
{
    Weapon,
    Helmet,
    Accessory,
    Boots,  // Legwear?

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

[CreateAssetMenu(menuName = "Gear/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    [SerializeField] private string itemName;   // Player facing name
    [SerializeField] private ItemID itemID;     // Internal ID

    [SerializeField] private InventoryItemSlot itemSlot;
    [SerializeField] private int baseCost = 10;

    [SerializeField] private string shortDescription;
    [TextArea(15,20)]
    [SerializeField] private string longDescription;

    [SerializeField] private Sprite icon;


    public string ItemName()
    {
        return itemName;
    }

    public ItemID ItemID()
    {
        return itemID;
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