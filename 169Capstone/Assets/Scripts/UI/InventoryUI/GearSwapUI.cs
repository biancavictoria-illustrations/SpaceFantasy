using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearSwapUI : MonoBehaviour
{
    public InventoryUI gearSwapInventoryUI;
    public Button replaceItemButton;
    public Button keepCurrentItemButton;

    private GameObject newItem;
    [SerializeField] private InventoryUIItemPanel newItemPanel;

    public void OnGearSwapUIOpen(GameObject item)
    {
        gearSwapInventoryUI.SetAllInventoryValues();

        newItem = item;
        newItemPanel.SetItemPanelValues(item);
        
        // TODO: in gearSwapInventoryUI, toggle on the item of type == new item type so that it starts open
        // for now, defaults to the top one being open
        if(gearSwapInventoryUI.itemPanels.Count == 0){
            Debug.LogError("No item panels found in gear swap inventory UI!");
        }
        gearSwapInventoryUI.OnInventoryOpen();
        gearSwapInventoryUI.CardToggle(gearSwapInventoryUI.itemPanels[0]);  // TODO: Even this default isn't working again...?
    }

    public void CloseGearSwapUI()
    {
        gearSwapInventoryUI.OnInventoryClose();
        InputManager.instance.ToggleCompareItemUI(false, null);    // Close the menu
    }

    public void OnNewItemSelect()
    {
        // TODO: Update inventory so that you now have the new item equipped
        // PlayerInventory.instance.EquipItem(ITEM TYPE, newItem);

        CloseGearSwapUI();
    }

    public void SetSwapUIInteractable(bool set)
    {
        replaceItemButton.interactable = set;
        keepCurrentItemButton.interactable = set;
        gearSwapInventoryUI.SetInventoryInteractable(set);
    }
}
