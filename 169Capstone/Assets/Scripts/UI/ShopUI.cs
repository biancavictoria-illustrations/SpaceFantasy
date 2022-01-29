using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public GameObject shopInventoryPanel;
    public GameObject shopCompareItemPanel;

    public GearSwapUI shopCompareUI;

    [SerializeField] private Button shopInventoryTopButton;
    [SerializeField] private List<ShopUIItemPanel> itemPanels = new List<ShopUIItemPanel>();
    
    [SerializeField] private Button leaveShopButton;
    [SerializeField] private Button compareCancelButton;
    [SerializeField] private TMP_Text purchaseButtonText;

    // Five items this shop has
    // [HideInInspector] public HashSet<InventoryUIItemPanel> itemInventory = new HashSet<InventoryUIItemPanel>();


    // TODO: open the shop
    public void OpenShopUI()
    {
        InputManager.instance.shopIsOpen = true;
        ToggleShopInventoryOn();

        SetShopUIValues();
    }

    private void SetShopUIValues()
    {
        // TODO
    }

    public void CloseShopUI()
    {
        InputManager.instance.shopIsOpen = false;
        shopInventoryPanel.SetActive(false);
        shopCompareItemPanel.SetActive(false);

        AlertTextUI.instance.EnableShopAlert();
    }

    public void ToggleShopInventoryOn()
    {
        shopInventoryPanel.SetActive(true);
        shopCompareItemPanel.SetActive(false);

        shopInventoryTopButton.Select();
    }

    public void ToggleShopCompareOn()
    {
        shopInventoryPanel.SetActive(false);
        shopCompareItemPanel.SetActive(true);

        compareCancelButton.Select();

        shopCompareUI.OnGearSwapUIOpen();     // TODO: Set the values for the item to compare to in shopCompareUI.OnGearSwapUIOpen();
        // purchaseButtonText.text = "Purchase " + "(" + COST + ")";
    }

    public void PurchaseItem()
    {
        Debug.Log("purchased item!");

        // TODO: Update inventory so that you now have the new item equipped

        // TODO: decrease currency

        ToggleShopInventoryOn();

        // TODO: purchased item NO LONGER AVAILABLE
    }

    public void SetShopUIInteractable(bool set)
    {
        // If the shop inventory is the active panel, set those buttons
        if(shopInventoryPanel.activeInHierarchy){
            leaveShopButton.interactable = set;
            foreach(ShopUIItemPanel panel in itemPanels){
                panel.GetComponent<Button>().interactable = set;
            }
            if(set){
                shopInventoryTopButton.Select();
            }
        }
        // If the compare item panel is active, set those buttons
        else{
            shopCompareUI.SetSwapUIInteractable(set);
            if(set){
                compareCancelButton.Select();
            }
        }
    }
}
