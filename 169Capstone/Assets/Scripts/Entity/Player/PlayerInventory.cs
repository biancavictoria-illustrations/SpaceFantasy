using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    // Gear dictionary contains keys of the enum item slot value and values of the actual item
    public Dictionary<InventoryItemSlot, Equipment> gear {get; private set;}

    [HideInInspector] public GameObject weaponModel;

    [Tooltip("For when you unequip items from your inventory.")]
    public GameObject dropItemPrefab;

    public int healthPotionQuantity {get; private set;}
    public int startingHealthPotionQuantity = 3;

    public int tempCurrency {get; private set;}
    public int permanentCurrency {get; private set;}

    public int totalPermanentCurrencySpent = 0;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }

        gear = new Dictionary<InventoryItemSlot, Equipment>();
        tempCurrency = 0;
        permanentCurrency = 0;
    }

    void Start()
    {
        // Set default values for all slots
        gear[InventoryItemSlot.Weapon] = null;
        gear[InventoryItemSlot.Accessory] = null;
        gear[InventoryItemSlot.Helmet] = null;
        gear[InventoryItemSlot.Legs] = null;
    }

    public void UseHealthPotion()
    {
        if(healthPotionQuantity == 0){
            // TODO: probably give UI feedback, like a little "no potions!" pop up
            return;
        }

        EntityHealth playerHealth = Player.instance.health;
        float healedHitPoints = playerHealth.maxHitpoints * (0.01f * Player.instance.stats.getHealingEfficacy());
        playerHealth.Heal(healedHitPoints);

        healthPotionQuantity--;
        InGameUIManager.instance.SetHealthPotionValue(healthPotionQuantity);
    }

    public void SetRunStartHealthPotionQuantity()
    {
        healthPotionQuantity = startingHealthPotionQuantity;
    }

    public void IncrementHealthPotionQuantity( int potionNum = 1 )
    {
        healthPotionQuantity += potionNum;
        InGameUIManager.instance.SetHealthPotionValue(healthPotionQuantity);
    }

    public void ClearHealthPotions()
    {
        healthPotionQuantity = 0;
        InGameUIManager.instance.SetHealthPotionValue(healthPotionQuantity);
    }

    public void EquipItem(InventoryItemSlot slot, Equipment item)
    {
        AlertTextUI.instance.DisableAlert();

        // If necessary, drop the PREVIOUS item on the ground
        UnequipItemSlot(slot);

        // Set the slot to now be the NEW item
        gear[slot] = item;
        
        // Update UI accordingly
        InGameUIManager.instance.SetGearItemUI(slot, item.data.equipmentBaseData.Icon());
    }

    public void UnequipItemSlot(InventoryItemSlot slot)
    {
        // If nothing's equipped, return
        if( !gear[slot] ){
            return;
        }

        // Instantiate the item to drop on the ground
        GameObject dropItem = Instantiate(dropItemPrefab, Player.instance.transform.position, Quaternion.identity);

        // Give it the data of your currently equipped item
        dropItem.GetComponent<GeneratedEquipment>().SetAllEquipmentData(gear[slot].data);

        // Drop it
        dropItem.GetComponent<DropTrigger>().DropItemModelIn3DSpace();
        
        // If it's a weapon, also remove the model stuck to your hand
        if(slot == InventoryItemSlot.Weapon){
            RemoveEquippedWeaponModel();
        }

        // Set value to null in the dictionary and clear the UI
        ClearItemSlot(slot);
        AlertTextUI.instance.EnableItemPickupAlert();
    }

    private void RemoveEquippedWeaponModel()
    {
        Destroy(weaponModel);
    }

    private void ClearItemSlot(InventoryItemSlot slot)
    {
        gear[slot] = null;
        InGameUIManager.instance.ClearItemUI(slot);
    }

    // Called when you die
    public void ClearRunInventory()
    {
        if(weaponModel){
            RemoveEquippedWeaponModel();
        }

        // Delete each item object and clear the inventory slot
        // (Can't loop because can't delete the things while iterating, stuff gets mad)
        if(gear[InventoryItemSlot.Weapon]){
            Destroy(gear[InventoryItemSlot.Weapon]);
            ClearItemSlot(InventoryItemSlot.Weapon);
        }
        if(gear[InventoryItemSlot.Accessory]){
            Destroy(gear[InventoryItemSlot.Accessory]);
            ClearItemSlot(InventoryItemSlot.Accessory);
        }
        if(gear[InventoryItemSlot.Legs]){
            Destroy(gear[InventoryItemSlot.Legs]);
            ClearItemSlot(InventoryItemSlot.Legs);
        }
        if(gear[InventoryItemSlot.Helmet]){
            Destroy(gear[InventoryItemSlot.Helmet]);
            ClearItemSlot(InventoryItemSlot.Helmet);
        }

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

    public void SpendPermanentCurrency(int amountSpent)
    {
        permanentCurrency -= amountSpent;
        totalPermanentCurrencySpent += amountSpent;
        InGameUIManager.instance.SetPermanentCurrencyValue(permanentCurrency);
    }

    public void ResetPermanentCurrency()
    {
        permanentCurrency += totalPermanentCurrencySpent;
        totalPermanentCurrencySpent = 0;
        InGameUIManager.instance.SetPermanentCurrencyValue(permanentCurrency);        
    }
}
