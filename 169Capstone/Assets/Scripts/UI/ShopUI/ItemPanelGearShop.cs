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

    public GameObject item {get; private set;}

    [SerializeField] private ShopUIGear shopUI;
    

    public void SetGearItemValues(GameObject item)
    {
        // SetBaseShopItemValues(iBaseCost, iName, iDesc);

        // itemIcon.sprite = iIcon;
        // itemSlot = iSlot;
        // itemSlotRarity.text = iRarity.ToString() + "/" + itemSlot.ToString();
        // enhancementCount.text = "Enhancement Count - " + iEnhancements;
    }

    public void OnItemClicked()
    {
        shopUI.activeCompareItem = this;
        shopUI.ToggleShopCompareOn();
    }

    public override void PurchaseItem()
    {
        base.PurchaseItem();

        PlayerInventory.instance.EquipItem(itemSlot, item);

        descriptionText.text = "<b><color=red>SOLD";
        itemCardButton.interactable = false;
    }
}
