using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public GameObject shopInventoryPanel;
    public GameObject shopCompareItemPanel;

    public GearSwapUI shopCompareUI;

    public SpeakerData shopkeeper;

    // Five items this shop has
    [HideInInspector] public HashSet<InventoryUIItemPanel> itemInventory = new HashSet<InventoryUIItemPanel>();


    // TODO: open the shop
    public void OpenShopUI()
    {
        InputManager.instance.shopIsOpen = true;
        ToggleInventoryOnCompareOff(true);

        SetShopUIValues();
    }

    private void SetShopUIValues()
    {

    }

    public void CloseShopUI()
    {
        InputManager.instance.shopIsOpen = false;
        shopInventoryPanel.SetActive(false);
        shopCompareItemPanel.SetActive(false);
    }

    // True sets shop inventory ACTIVE and compare INACTIVE
    // False sets shop inventory INACTIVE and compare ACTIVE
    public void ToggleInventoryOnCompareOff(bool set)
    {
        shopInventoryPanel.SetActive(set);
        shopCompareItemPanel.SetActive(!set);

        if(!set){
            // TODO: Set the values for the item to compare to in shopCompareUI.OnGearSwapUIOpen();
        }
    }

    public void PurchaseItem()
    {
        // TODO: Update inventory so that you now have the new item equipped

        // TODO: decrease currency

        ToggleInventoryOnCompareOff(true);

        // TODO: purchased item NO LONGER AVAILABLE
    }
}
