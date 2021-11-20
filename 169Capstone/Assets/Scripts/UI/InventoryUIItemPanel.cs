using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// TODO: set these to whatever we're actually calling them in game
public enum InventoryItemType
{
    Helmet,
    Accessory,
    Boots,
    Weapon,
    Potion,

    enumSize
}

public class InventoryUIItemPanel : MonoBehaviour
{
    // Should be one of each
    public InventoryItemType itemType;

    public TMP_Text itemSlotTitle;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public Image itemIcon;

    void Start()
    {
        itemSlotTitle.text = itemType.ToString();
    }

    public void SetItemPanelValues(string iName, string iDescription, Sprite iIcon)
    {
        itemName.text = iName;
        itemDescription.text = iDescription;
        itemIcon.sprite = iIcon;
    }
}
