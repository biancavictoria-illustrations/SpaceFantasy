using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPanelGearShop : ItemPanelShopUI
{
    [SerializeField] private Image itemIcon;
    private InventoryItemSlot itemSlot;
    [SerializeField] private TMP_Text itemSlotRarity;
    [SerializeField] private TMP_Text enhancementCount;

    private GameObject item;

    [SerializeField] private ShopUIGear shopUI;
    

    // just pass in the item instead and set all the values accordingly
    public void SetGearItemValues(int iBaseCost, string iName, string iDesc, Sprite iIcon, InventoryItemSlot iSlot, ItemRarity iRarity, string iEnhancements)
    {
        SetBaseShopItemValues(iBaseCost, iName, iDesc);

        itemIcon.sprite = iIcon;
        itemSlot = iSlot;
        itemSlotRarity.text = iRarity.ToString() + "/" + itemSlot.ToString();
        enhancementCount.text = "Enhancement Count - " + iEnhancements;
    }

    public void OnItemClicked()
    {
        shopUI.activeCompareItem = this;
        shopUI.ToggleShopCompareOn();
    }

    public override void PurchaseItem()
    {
        base.PurchaseItem();

        // TODO: Add item to inventory and stuff

        // TODO: purchased item shows "sold" and is NO LONGER AVAILABLE
        descriptionText.text = "<b><color=red>SOLD";
        itemCardButton.interactable = false;
    }
}
