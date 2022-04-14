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

    private const float rarityMultiplierBase = 1.2f;
    private const float costPowerValue = 1.25f;
    private const float timeFactor = 0.0606f;
    

    public void SetGearItemValues(GeneratedEquipment _item)
    {
        item = _item;
        EquipmentBaseData data = item.data.equipmentBaseData;
        rarity = item.data.rarity;

        SetBaseShopItemValues(data.BaseCost(), data.ItemName(), data.ShortDescription());

        itemIcon.sprite = data.Icon();
        itemSlot = data.ItemSlot();
        itemSlotRarity.text = item.data.rarity.ToString() + "/" + itemSlot.ToString();
        enhancementCount.text = "Enhancement Count - " + item.data.enhancementCount;
    }

    public void OnItemClicked()
    {
        shopUI.activeCompareItem = this;
        hoverAlerts.EnableAlert(panelPos, false);
        shopUI.ToggleShopCompareOn(PlayerInventory.instance.tempCurrency - currentCostValue > 0);
    }

    public override void PurchaseItem()
    {
        // Purchase the item
        base.PurchaseItem();

        if(PlayerInventory.instance.tempCurrency - currentCostValue < 0){
            return;
        }
        
        // Generate the actual item object in the scene (from the item data)
        item.EquipGeneratedItem();

        descriptionText.text = "<b><color=red>SOLD";    // TODO: Make this permanent (it's not at the moment)
        itemCardButton.interactable = false;
    }
}
