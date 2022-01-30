using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIGear : ShopUI
{
    public GameObject shopCompareItemPanel;
    public GearSwapUI shopCompareUI;
    [HideInInspector] public ItemPanelGearShop activeCompareItem;

    [SerializeField] private Button compareCancelButton;
    [SerializeField] private TMP_Text purchaseButtonText;


    public override void OpenShopUI()
    {
        base.OpenShopUI();
        SetShopUIValues();
    }
  
    public override void CloseShopUI()
    {
        base.CloseShopUI();
        shopCompareItemPanel.SetActive(false);
    }

    private void SetShopUIValues()
    {
        // TODO
    }

    public void ToggleShopInventoryOn()
    {
        shopInventoryPanel.SetActive(true);
        shopCompareItemPanel.SetActive(false);
        InGameUIManager.instance.ToggleInGameGearIconPanel(true);

        shopInventoryTopButton.Select();
    }

    public void ToggleShopCompareOn()
    {
        shopInventoryPanel.SetActive(false);
        shopCompareItemPanel.SetActive(true);
        InGameUIManager.instance.ToggleInGameGearIconPanel(false);

        compareCancelButton.Select();

        if(activeCompareItem){
            shopCompareUI.OnGearSwapUIOpen();     // TODO: Set the values for the item to compare to in shopCompareUI.OnGearSwapUIOpen();
            purchaseButtonText.text = "Purchase " + "(" + activeCompareItem.currentCostValue + ")";
        }
        else{
            Debug.LogError("Tried to open compare UI without an active shop item to compare to!");
        }
    }

    public void OnCancelClicked()
    {
        ToggleShopInventoryOn();
        activeCompareItem = null;
    }

    public void OnPurchaseItemClicked()
    {
        activeCompareItem.PurchaseItem();
        OnCancelClicked();
    }

    public override void SetShopUIInteractable(bool set)
    {
        base.SetShopUIInteractable(set);
        if(shopCompareItemPanel.activeInHierarchy){
            shopCompareUI.SetSwapUIInteractable(set);
            if(set){
                compareCancelButton.Select();
            }
        }
    }
}