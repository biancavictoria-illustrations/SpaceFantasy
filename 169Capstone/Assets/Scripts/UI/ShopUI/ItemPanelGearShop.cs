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
        ToggleElectrumIconActive(itemIsAvailable);
        if(!itemIsAvailable){
            return;
        }

        item = _item;
        EquipmentBaseData baseData = item.equipmentBaseData;
        rarity = item.rarity;

        string itemNameWithColor = "<color=" + UIUtils.GetColorFromRarity(rarity) + ">" + baseData.ItemName() + "</color>";

        SetBaseShopItemValues(baseData.BaseCost(), itemNameWithColor, baseData.ShortDescription());

        itemIcon.sprite = baseData.Icon();
        itemSlot = baseData.ItemSlot();

        string statString = "";
        if(_item.equipmentBaseData.PrimaryStat() != PlayerStatName.size){
            statString = _item.equipmentBaseData.PrimaryStat().ToString();
        }
        itemSlotRarity.text = item.rarity.ToString() + " " + statString + " " + itemSlot.ToString();

        enhancementCount.text = "<color=" + InGameUIManager.CYAN_COLOR + ">Item Rating: " + GetItemRating(_item) + "</color>";
    }

    private int GetItemRating(GeneratedEquipment _item)
    {
        int itemRating = 0;

        // For each secondary line that this item has, add 1 to the rating
        for(int i = 0; i < (int)StatType.STRDamage; i++){
            if( _item.GetSecondaryLineValueFromStatType((StatType)i) > 0f ){
                itemRating++;
            }
        }

        // At this point if > 0, there ARE secondary lines, which means add # of enhancements as well
        if(itemRating > 0){
            itemRating += _item.enhancementCount;
        }

        // If NOT a Common Weapon, this item has a primary line -> add 1 for that
        if(_item.primaryLineValue > 0f){
            itemRating++;
        }

        return itemRating;
    }

    public void OnItemClicked()
    {
        shopUI.activeCompareItem = this;
        hoverAlerts.EnableAlert(panelPos, false);
        shopUI.ToggleShopCompareOn(PlayerInventory.instance.tempCurrency - currentCostValue >= 0);
    }

    public override void PurchaseItem()
    {
        // Purchase the item
        base.PurchaseItem();
        
        // Generate the actual item object in the scene (from the item data)
        item.EquipGeneratedItem();

        descriptionText.text = "<b><color=" + InGameUIManager.MAGENTA_COLOR + ">SOLD";
        itemCardButton.interactable = false;
        itemIsAvailable = false;
        ToggleElectrumIconActive(false);
    }
}
