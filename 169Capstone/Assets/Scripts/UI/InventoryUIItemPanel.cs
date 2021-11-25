using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    Epic,
    Legendary
}

public class InventoryUIItemPanel : MonoBehaviour
{
    // Should be one of each
    [SerializeField] private InventoryItemSlot itemSlot;
    [HideInInspector] public ItemRarity rarity;

    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemTypeRarity;
    public TMP_Text itemDescription;

    public GameObject descriptionPanel;

    public void SetItemPanelValues(string iName, ItemRarity iRarity, string description, Sprite icon)
    {
        itemName.text = iName;
        rarity = iRarity;
        itemDescription.text = description;
        itemIcon.sprite = icon;

        itemTypeRarity.text = "(" + iRarity.ToString() + " " + itemSlot.ToString() + ")";
    }
}
