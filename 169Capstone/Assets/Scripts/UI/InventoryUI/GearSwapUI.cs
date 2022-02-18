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
        gearSwapInventoryUI.SetInventoryItemValues();   // Only set item values, no stat values (is this even necessary? it might be called already)

        newItem = item;
        newItemPanel.SetItemPanelValues(item.data);
        
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

    // TODO: fix this
    public void CloseGearSwapUI()
    {
        // If this is a drop item, close the UI (if this is a shop, it's handled in the shop)
        // if( DropTrigger.ActiveGearDrop && DropTrigger.ActiveGearDrop == newItem ){
            gearSwapInventoryUI.OnInventoryClose();
            InputManager.instance.ToggleCompareItemUI(false, null);
        // }
    }

    // This might just work for both shop and drops!
    public void OnNewItemSelect()
    {
        newItem.EquipGeneratedItem();
        CloseGearSwapUI();
    }

    public void SetSwapUIInteractable(bool set)
    {
        replaceItemButton.interactable = set;
        keepCurrentItemButton.interactable = set;
        gearSwapInventoryUI.SetInventoryInteractable(set);
    }
}
