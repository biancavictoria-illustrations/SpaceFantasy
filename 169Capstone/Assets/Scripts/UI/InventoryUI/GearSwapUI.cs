using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearSwapUI : MonoBehaviour
{
    public InventoryUI gearSwapInventoryUI;
    public Button replaceItemButton;
    public Button keepCurrentItemButton;

    public static GeneratedEquipment newItem {get; private set;}
    [SerializeField] public InventoryUIItemPanel newItemPanel;

    public void OnGearSwapUIOpen(GeneratedEquipment item)
    {
        // Deal with new item stuff first because the inventory needs this stuff to be set to access it (for comparison values)
        newItem = item;
        newItemPanel.SetItemPanelValues(item, InventoryUIItemPanel.ItemPanelType.NewItemToCompare);
        newItemPanel.SetExpandedDescription(true);
        
        gearSwapInventoryUI.SetInventoryItemValues(item.equipmentBaseData.ItemSlot());   // Only set item values, no stat values
        
        if(gearSwapInventoryUI.itemPanels.Count == 0){
            Debug.LogError("No item panels found in gear swap inventory UI!");
        }
        gearSwapInventoryUI.SelectTopPanel();

        // If you have an item equipped in that slot, expand that one on open
        if(PlayerInventory.instance.gear[item.equipmentBaseData.ItemSlot()] != null){
            ExpandItemOfType( item.equipmentBaseData.ItemSlot() );
        }
    }

    private void ExpandItemOfType(InventoryItemSlot slot)
    {
        if(!PlayerInventory.instance.gear[slot]){
            Debug.Log("No item of slot type " + slot.ToString() + " equipped; not expanding item panel.");
            return;
        }

        foreach(InventoryUIItemPanel panel in gearSwapInventoryUI.itemPanels){
            if(panel.GetItemSlot() == slot){
                gearSwapInventoryUI.ManuallyToggleCard(panel);
                return;
            }
        }
    }

    public void CloseGearSwapUI()
    {
        // If this is a drop item, close the UI (if this is a shop, it's handled in the shop)
        // Don't actually think this is necessary here? but leaving it as a precaution to try not to screw up shops
        if( InGameUIManager.instance.gearSwapIsOpen ){
            // Collapse all item panels
            gearSwapInventoryUI.OnInventoryClose();

            // Tell the input manager we closed the UI
            InputManager.instance.ToggleCompareItemUI(false, null);
            InputManager.instance.RunGameTimer(true);
        }
    }

    public void OnNewItemSelect()
    {
        newItem.EquipGeneratedItem();
        CloseGearSwapUI();
        
        // Deal with UI alerts
        if(PlayerInventory.hasPickedSomethingUpThisRun){
            AlertTextUI.instance.DisablePrimaryAlert();
        }        
        else{
            PlayerInventory.hasPickedSomethingUpThisRun = true;
            AlertTextUI.instance.EnableOpenInventoryAlert();
            StartCoroutine(AlertTextUI.instance.RemovePrimaryAlertAfterSeconds());
        }

        // If this is a spawn room weapon, drop the force fields
        if( SpawnRoomForceFieldUnlockItem.activeForceFieldUnlockItem )
            SpawnRoomForceFieldUnlockItem.activeForceFieldUnlockItem.UnlockForceFieldsOnPickUp();
    }

    public void SetSwapUIInteractable(bool set)
    {
        replaceItemButton.interactable = set;
        keepCurrentItemButton.interactable = set;
        gearSwapInventoryUI.SetInventoryInteractable(set);
    }
}
