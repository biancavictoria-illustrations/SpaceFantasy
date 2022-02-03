using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    // Gear dictionary contains keys of the enum item slot value and values of the actual item
    public Dictionary<InventoryItemSlot, GeneratedEquipment> gear {get; private set;}

    public int healthPotionQuantity {get; private set;}

    public int tempCurrency {get; private set;}
    public int permanentCurrency {get; private set;}    // TODO: NOT set to not destroy on load so this might be a problem here

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }

        gear = new Dictionary<InventoryItemSlot, GeneratedEquipment>();
        // Set default values for all slots
    }

    void Start()
    {
        ClearInventory();   // TODO: Move this somewhere else!!! Might cause problems moving between rooms/loading save data?
    }

    public void UseHealthPotion()
    {
        if(healthPotionQuantity == 0){
            // TODO: probably give UI feedback, like a little "no potions!" pop up
            return;
        }

        // TODO: Move this to Player script probably so that we're not getting so many components
        float healedHitPoints = GetComponent<EntityHealth>().maxHitpoints * (0.01f * GetComponent<PlayerStats>().getHealingEfficacy());
        GetComponent<EntityHealth>().Heal(healedHitPoints);

        healthPotionQuantity--;
        InGameUIManager.instance.SetHealthPotionValue(healthPotionQuantity);
    }

    public void PurchaseHealthPotion()
    {
        healthPotionQuantity++;
        InGameUIManager.instance.SetHealthPotionValue(healthPotionQuantity);
    }

    public void ClearHealthPotions()
    {
        healthPotionQuantity = 0;
        InGameUIManager.instance.SetHealthPotionValue(healthPotionQuantity);
    }

    public void EquipItem(InventoryItemSlot slot, GeneratedEquipment item)
    {
        gear[slot] = item;
        InGameUIManager.instance.SetGearItemUI(slot, item);
    }

    public void UnequipItem(InventoryItemSlot slot)
    {
        gear[slot] = null;
        InGameUIManager.instance.ClearItemUI(slot);
    }

    // Called when you die (putting this here means we need to put resetting your health somewhere else)
    public void ClearInventory()
    {
        UnequipItem(InventoryItemSlot.Weapon);
        UnequipItem(InventoryItemSlot.Helmet);
        UnequipItem(InventoryItemSlot.Accessory);
        UnequipItem(InventoryItemSlot.Boots);

        SetTempCurrency(0);
        ClearHealthPotions();
    }

    public void SetTempCurrency(int value)
    {
        tempCurrency = value;
        InGameUIManager.instance.SetTempCurrencyValue(tempCurrency);
    }

    public void SetPermanentCurrency(int value)
    {
        permanentCurrency = value;
        InGameUIManager.instance.SetPermanentCurrencyValue(permanentCurrency);
    }
}
