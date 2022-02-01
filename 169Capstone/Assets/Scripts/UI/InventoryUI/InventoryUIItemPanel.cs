using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUIItemPanel : MonoBehaviour
{
    [SerializeField] private InventoryItemSlot itemSlot;
    [HideInInspector] public ItemRarity rarity;

    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemTypeRarity;
    public TMP_Text itemDescription;

    private string shortDescription = "";    // Single line
    private string expandedDescription = ""; // Detailed additions

    public GameObject descriptionPanel;
    public FlexibleGridLayout textGrid;
    public HorizontalLayoutGroup horizontalLayoutGroup;

    public void SetItemPanelValues(GameObject item)
    {
        if(item == null){
            SetDefaultItemPanelValues();
            return;
        }

        // TODO: Set based on item values

        // itemName.text = iName;
        // rarity = iRarity;

        // itemTypeRarity.text = iRarity.ToString() + " " + itemSlot.ToString();

        // shortDescription = shortDesc;
        // expandedDescription = expandedDesc;
        // itemDescription.text = shortDesc;
        
        // itemIcon.sprite = icon;
        
        GetComponent<Toggle>().interactable = true;
    }

    private void SetDefaultItemPanelValues()
    {
        itemName.text = "";
        rarity = ItemRarity.none;

        itemTypeRarity.text = itemSlot.ToString();

        shortDescription = "EMPTY";
        expandedDescription = "";
        itemDescription.text = shortDescription;

        // TODO: itemIcon.sprite = DEFAULT ICON;

        GetComponent<Toggle>().interactable = false;
    }

    public InventoryItemSlot GetItemSlot()
    {
        return itemSlot;
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
