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

    // TODO: Set to private after testing
    // [TextArea(5,10)]
    private string shortDescription = "";    // Single line
    // [TextArea(5,10)]
    private string expandedDescription = ""; // Detailed additions

    public GameObject descriptionPanel;
    public FlexibleGridLayout textGrid;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    public void SetItemPanelValues(string iName, ItemRarity iRarity, string shortDesc, string expandedDesc, Sprite icon)
    {
        itemName.text = iName;
        rarity = iRarity;

        itemTypeRarity.text = "(" + iRarity.ToString() + " " + itemSlot.ToString() + ")";

        shortDescription = shortDesc;
        expandedDescription = expandedDesc;
        itemDescription.text = shortDesc;
        
        itemIcon.sprite = icon;
    }

    public void SetExpandedDescription(bool set)
    {
        if(set){
            itemDescription.text = shortDescription + expandedDescription;
        }
        else{
            itemDescription.text = shortDescription;
        }
    }
}
