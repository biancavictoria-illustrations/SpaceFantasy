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
    public void SetInventoryCompareItemUIActive(EquipmentBase item)
    {
        inventoryCompareItemPanel.SetActive(true);

        // Set values
        SetOtherItemValues();
        SetEquippedItemValues(item.equipmentData.ItemSlot());
    }

    public void CloseInventoryUI()
    {
        // Clear
        inventoryCompareItemPanel.SetActive(false);
    }

    // TODO: Take in an item and fill in the values
    public void SetOtherItemValues()
    {
        
    }

    public void SetEquippedItemValues(InventoryItemSlot itemType)
    {
        equippedItemSlotTitle.text = itemType.ToString();
        // TODO: Get the equipped item of the correct type and fill in the values
    }
}
