using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryCompareItemUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCompareItemPanel;

    [SerializeField] private TMP_Text equippedItemSlotTitle;
    [SerializeField] private TMP_Text equippedItemName;
    [SerializeField] private TMP_Text equippedItemDescription;
    [SerializeField] private Image equippedItemIcon;

    [SerializeField] private TMP_Text otherItemSlotTitle;
    [SerializeField] private TMP_Text otherItemName;
    [SerializeField] private TMP_Text otherItemDescription;
    [SerializeField] private Image otherItemIcon;


    // TODO: When the player checks an item on the ground, call this
    public void SetInventoryCompareItemUIActive()  // TODO: Take in an item
    {
        inventoryCompareItemPanel.SetActive(true);
        // Set values
        SetOtherItemValues();
        // SetEquippedItemValues();    // TODO: Pass in the other item's item type

        // Pause game
    }

    public void CloseInventoryUI()
    {
        // Clear
        inventoryCompareItemPanel.SetActive(false);
        // Unpause game
    }

    // TODO: Take in an item and fill in the values
    public void SetOtherItemValues()
    {
        
    }

    public void SetEquippedItemValues(InventoryItemType itemType)
    {
        equippedItemSlotTitle.text = itemType.ToString();
        // TODO: Get the equipped item of the correct type and fill in the values
    }
}
