using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: set these to whatever we're actually calling them in game
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
    none
}

[CreateAssetMenu(menuName = "ScriptableObjects/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    [SerializeField] private string itemName;

    [SerializeField] private InventoryItemSlot itemSlot;

    [SerializeField] private string shortDescription;
    [TextArea(15,20)]
    [SerializeField] private string longDescription;

    [SerializeField] private Sprite icon;


    public string ItemName()
    {
        return itemName;
    }

    public InventoryItemSlot ItemSlot()
    {
        return itemSlot;
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
