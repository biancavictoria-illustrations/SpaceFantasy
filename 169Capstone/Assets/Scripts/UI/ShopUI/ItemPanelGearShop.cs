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
        if(!itemIsAvailable){
            // descriptionText.text = "<b><color=red>SOLD";
            // itemCardButton.interactable = false;
            return;
        }

        item = _item;
        EquipmentBaseData baseData = item.equipmentBaseData;
        rarity = item.rarity;

        SetBaseShopItemValues(baseData.BaseCost(), baseData.ItemName(), baseData.ShortDescription());

        itemIcon.sprite = baseData.Icon();
        itemSlot = baseData.ItemSlot();
        itemSlotRarity.text = item.rarity.ToString() + "/" + itemSlot.ToString();
        enhancementCount.text = "Enhancement Count - " + item.enhancementCount;
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
        
        // Generate the actual item object in the scene (from the item data)
        item.EquipGeneratedItem();

        descriptionText.text = "<b><color=red>SOLD";
        itemCardButton.interactable = false;
        itemIsAvailable = false;
    }
}
