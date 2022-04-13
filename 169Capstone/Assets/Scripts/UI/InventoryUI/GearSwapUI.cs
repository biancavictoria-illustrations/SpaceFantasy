using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearSwapUI : MonoBehaviour
{
    public InventoryUI gearSwapInventoryUI;
    public Button replaceItemButton;
    public Button keepCurrentItemButton;

    private GeneratedEquipment newItem;
    [SerializeField] private InventoryUIItemPanel newItemPanel;

    public void OnGearSwapUIOpen(GeneratedEquipment item)
    {
        gearSwapInventoryUI.SetInventoryItemValues();   // Only set item values, no stat values

        newItem = item;
        newItemPanel.SetItemPanelValues(item.data);
        newItemPanel.SetExpandedDescription(true);
        
        if(gearSwapInventoryUI.itemPanels.Count == 0){
            Debug.LogError("No item panels found in gear swap inventory UI!");
        }
        gearSwapInventoryUI.OnInventoryOpen();

        // If you have an item equipped in that slot, expand that one on open
        if(PlayerInventory.instance.gear[item.data.equipmentBaseData.ItemSlot()] != null){
            ExpandItemOfSameType();
        }
    }

    private void ExpandItemOfSameType()
    {
        InventoryItemSlot slot = newItem.data.equipmentBaseData.ItemSlot();

        if(!PlayerInventory.instance.gear[slot]){
            Debug.Log("No item of slot type " + slot.ToString() + " equipped; not expanding item panel.");
            return;
        }

        foreach(InventoryUIItemPanel panel in gearSwapInventoryUI.itemPanels){
            if(panel.GetItemSlot() == slot){
                gearSwapInventoryUI.CardToggle(panel);
            }
        }
    }

    public void CloseGearSwapUI()
    {
        // If this is a drop item, close the UI (if this is a shop, it's handled in the shop)
        // Don't actually think this is necessary here? but leaving it as a precaution to try not to screw up shops
        if( InGameUIManager.instance.gearSwapIsOpen ){
            gearSwapInventoryUI.OnInventoryClose();
            InputManager.instance.ToggleCompareItemUI(false, null);
        }
        InputManager.instance.RunGameTimer(true);
    }

    public void OnNewItemSelect()
    {
        newItem.EquipGeneratedItem();
        CloseGearSwapUI();
        AlertTextUI.instance.DisableAlert();
    }

    public void SetSwapUIInteractable(bool set)
    {
        replaceItemButton.interactable = set;
        keepCurrentItemButton.interactable = set;
        gearSwapInventoryUI.SetInventoryInteractable(set);
    }
}
