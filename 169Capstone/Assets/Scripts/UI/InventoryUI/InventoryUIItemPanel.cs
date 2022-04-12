﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUIItemPanel : MonoBehaviour
{
    // public enum StringVariableValues{
    //     STR,
    //     DEX,
    //     INT,
    //     WIS,
    //     CON,
    //     CHA
    // }

    [SerializeField] private InventoryItemSlot itemSlot;
    [HideInInspector] public ItemRarity rarity;

    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemTypeRarity;
    public TMP_Text itemDescription;

    private string shortDescription = "";    // 1-2 lines
    private string expandedDescription = ""; // Detailed additions

    public GameObject descriptionPanel;
    public FlexibleGridLayout textGrid;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    [SerializeField] private Toggle toggle;

    public void SetItemPanelValues(SpawnedEquipmentData item)
    {
        EquipmentBaseData data = item.equipmentBaseData;

        itemName.text = data.ItemName();
        rarity = item.rarity;

        itemTypeRarity.text = rarity.ToString() + " " + data.ItemSlot().ToString();

        shortDescription = data.ShortDescription();
        expandedDescription = data.LongDescription();
        itemDescription.text = shortDescription;

        itemIcon.sprite = data.Icon();
        itemIcon.preserveAspect = true;
        
        // Check bc compare item panel doesn't have a toggle
        if(toggle){
            toggle.interactable = true;
        }
    }

    public void SetDefaultItemPanelValues()
    {
        itemName.text = "";
        rarity = ItemRarity.enumSize;

        itemTypeRarity.text = itemSlot.ToString();

        shortDescription = "EMPTY";
        expandedDescription = "";
        itemDescription.text = shortDescription;

        itemIcon.sprite = InGameUIManager.instance.GetDefaultItemIconForSlot(itemSlot);
        itemIcon.preserveAspect = true;

        if(toggle){
            toggle.interactable = false;
        }
    }

    public InventoryItemSlot GetItemSlot()
    {
        return itemSlot;
    }

    public void SetExpandedDescription(bool set)
    {
        if(set){
            itemDescription.text = expandedDescription;
        }
        else{
            itemDescription.text = shortDescription;
        }
    }
}
