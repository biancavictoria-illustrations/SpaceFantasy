using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearSwapUI : MonoBehaviour
{
    public InventoryUI gearSwapInventoryUI;

    [SerializeField] private InventoryUIItemPanel newItemPanel;

    [HideInInspector] public bool newItemSelected;

    void Start()
    {
        newItemSelected = false;
    }

    // TODO: Passes in the item to compare to
    public void OnGearSwapUIOpen()
    {
        // gearSwapInventoryUI.SetAllInventoryValues();

        // TODO: Set newItemPanel values according to the item passed in
        
        // TODO: in gearSwapInventoryUI, toggle on the item of type == new item type so that it starts open
        // for now, defaults to the top one being open
        if(gearSwapInventoryUI.itemPanels.Count == 0){
            Debug.LogError("No item panels found in gear swap inventory UI!");
        }
        gearSwapInventoryUI.CardToggle(gearSwapInventoryUI.itemPanels[0]);
    }

    public void OnGearSwapUIClose()
    {
        newItemSelected = false;
    }

    public void OnNewItemSelect()
    {
        newItemSelected = true;
        // TODO: Update inventory so that you now have the new item equipped

        InputManager.instance.ToggleCompareItemUI(false);    // Close the menu
    }
}
