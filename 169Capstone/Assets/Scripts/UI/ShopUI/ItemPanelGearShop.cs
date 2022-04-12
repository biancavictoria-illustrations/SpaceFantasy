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
        shopUI.ToggleShopCompareOn();
    }

    public override void PurchaseItem()
    {
        // Purchase the item
        base.PurchaseItem();

        if(PlayerInventory.instance.tempCurrency - currentCostValue < 0){
            // TODO: Make the purchase button not interactable and instead it says something like "not enough electrum!"
            return;
        }
        
        // Generate the actual item object in the scene (from the item data)
        item.EquipGeneratedItem();

        descriptionText.text = "<b><color=red>SOLD";    // TODO: Make this permanent (it's not at the moment)
        itemCardButton.interactable = false;
    }

    protected override void CalculateCurrentCost()
    {
        float cost = baseCost;      // Set base cost

        // Set the rarity multiplier (rarity multiplier base to a power of the ItemRarity value)
        float rarityMultiplier = Mathf.Pow(rarityMultiplierBase, (int)item.data.rarity);

        // Set coeff to (time factor * time in min) * stage factor
        int playerFactor = 1;
        float timeInMin = 0;        // TODO: Set to time in min
        float stageFactor = 1f;     // TODO: Set to stage factor
        float coeff = (playerFactor + timeInMin * timeFactor) * stageFactor;

        // Raise coeff to the power of the costPowerValue
        coeff = Mathf.Pow(coeff,costPowerValue);

        cost = cost * coeff * rarityMultiplier;     // Multiply base cost by coeff and rarity multiplier
        currentCostValue = (int)Mathf.Floor(cost);  // Get int using Floor to round
    }
}
