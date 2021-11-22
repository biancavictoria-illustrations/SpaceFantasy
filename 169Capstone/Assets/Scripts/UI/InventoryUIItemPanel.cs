using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// TODO: set these to whatever we're actually calling them in game
public enum InventoryItemSlot
{
    Helmet,
    Accessory,
    Boots,
    Weapon,
    Potion,

    enumSize
}

// TODO: set these to whatever we're actually calling them in game
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

public class InventoryUIItemPanel : MonoBehaviour
{
    // Should be one of each
    [SerializeField] private InventoryItemSlot itemSlot;
    [HideInInspector] public ItemRarity rarity;

    public TMP_Text itemName;
    public TMP_Text itemTypeRarity;
    public TMP_Text itemDescription;

    void Start()
    {
        itemTypeRarity.text = "(" + rarity.ToString() + " " + itemSlot.ToString() + ")";
    }

    public void SetItemPanelValues(string iName, string iDescription)
    {
        itemName.text = iName;
        itemDescription.text = iDescription;
    }
}
