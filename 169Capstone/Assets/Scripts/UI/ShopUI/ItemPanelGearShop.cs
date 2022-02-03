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

    public GeneratedEquipment item {get; private set;}

    [SerializeField] private ShopUIGear shopUI;
    

    public void SetGearItemValues(GeneratedEquipment _item)
    {
        item = _item;
        EquipmentData data = item.equipmentData;

        item.CalculateCurrentCost();
        SetBaseShopItemValues(item.currentCost, data.ItemName(), data.ShortDescription());

        itemIcon.sprite = data.Icon();
        itemSlot = data.ItemSlot();
        itemSlotRarity.text = item.rarity.ToString() + "/" + itemSlot.ToString();
        enhancementCount.text = "Enhancement Count - " + item.enhancementCount;
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
