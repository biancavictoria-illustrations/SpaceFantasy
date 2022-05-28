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

    public int tempCurrency {get; private set;}
    public int permanentCurrency {get; private set;}

    [HideInInspector] public static bool hasPickedSomethingUpThisRun = false;

    public static bool hasCaptainsLog = true;

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
    }

    void Start()
    {
        // Set default values for all slots
        gear[InventoryItemSlot.Weapon] = null;
        gear[InventoryItemSlot.Accessory] = null;
        gear[InventoryItemSlot.Helmet] = null;
        gear[InventoryItemSlot.Legs] = null;
    }

    public void InitializeInventoryValuesOnNewGame()
    {        
        permanentCurrency = 0;
    }

    public void UseHealthPotion()
    {
        if(healthPotionQuantity == 0){
            // TODO: probably give UI feedback, like a little "no potions!" pop up
            return;
        }

        EntityHealth playerHealth = Player.instance.health;
        float healedHitPoints = playerHealth.maxHitpoints * Player.instance.stats.getHealingEfficacy();
        playerHealth.Heal(healedHitPoints);

        healthPotionQuantity--;
        InGameUIManager.instance.SetHealthPotionValue(healthPotionQuantity);
    }

    public void SetRunStartHealthPotionQuantity()
    {
        healthPotionQuantity = PermanentUpgradeManager.instance.startingHealthPotionQuantity;
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
        // If necessary, drop the PREVIOUS item on the ground
        UnequipItemSlot(slot);

        // Set the slot to now be the NEW item
        gear[slot] = item;
        
        // Update UI accordingly
        InGameUIManager.instance.SetGearItemUI(slot, item.data.equipmentBaseData.Icon());

        // Add your new passive upgrades
        SetStatBonusesFromItem(item.data);

        // If necessary, unlock journal entires
        if(item.data.equipmentBaseData.JournalEntriesUnlocked()?.Length > 0){
            GameManager.instance.journalContentManager.UnlockJournalEntry(item.data.equipmentBaseData.JournalEntriesUnlocked());
        }
    }

    private void SetStatBonusesFromItem(GeneratedEquipment itemData)
    {
        // Primary line
        if(itemData.primaryLineValue > 0){
            // If %
            if(itemData.equipmentBaseData.PrimaryItemLine() == StatType.HitPoints || (int)itemData.equipmentBaseData.PrimaryItemLine() >= (int)StatType.STRDamage){
                Player.instance.stats.SetBonusForStat( itemData.equipmentBaseData, itemData.equipmentBaseData.PrimaryItemLine(), EntityStats.BonusType.multiplier, itemData.primaryLineValue );
                CheckForHealthBarUpdate(itemData.equipmentBaseData.PrimaryItemLine());
            }
            // If flat
            else{
                Player.instance.stats.SetBonusForStat( itemData.equipmentBaseData, itemData.equipmentBaseData.PrimaryItemLine(), EntityStats.BonusType.flat, itemData.primaryLineValue );
            }
        }

        // Secondary line
        for(int i = 0; i < EntityStats.numberOfSecondaryLineOptions; i++){
            float bonusValue = itemData.GetLineValueFromStatType((StatType)i);
            if(bonusValue != 0){
                Player.instance.stats.SetBonusForStat( itemData.equipmentBaseData, (StatType)i, EntityStats.BonusType.flat, bonusValue );
                CheckForHealthBarUpdate((StatType)i);
            }
        }
    }

    private void CheckForHealthBarUpdate(StatType type)
    {
        if( type == StatType.HitPoints ){
            Player.instance.health.UpdateHealthOnUpgrade();
        }
    }

    private void RemoveStatBonusesFromItem(GeneratedEquipment itemData)
    {
        // Primary line
        if(itemData.primaryLineValue > 0){
            // If %
            if(itemData.equipmentBaseData.PrimaryItemLine() == StatType.HitPoints || (int)itemData.equipmentBaseData.PrimaryItemLine() >= (int)StatType.STRDamage){
                Player.instance.stats.SetBonusForStat( itemData.equipmentBaseData, itemData.equipmentBaseData.PrimaryItemLine(), EntityStats.BonusType.multiplier, 0 );
                CheckForHealthBarUpdate(itemData.equipmentBaseData.PrimaryItemLine());
            }
            // If flat
            else{
                Player.instance.stats.SetBonusForStat( itemData.equipmentBaseData, itemData.equipmentBaseData.PrimaryItemLine(), EntityStats.BonusType.flat, 0 );
            }
        }

        // Secondary line
        for(int i = 0; i < EntityStats.numberOfSecondaryLineOptions; i++){
            float bonusValue = itemData.GetLineValueFromStatType((StatType)i);
            if(bonusValue != 0){
                Player.instance.stats.SetBonusForStat( itemData.equipmentBaseData, (StatType)i, EntityStats.BonusType.flat, 0 );
                CheckForHealthBarUpdate((StatType)i);
            }
        }
    }

    public void UnequipItemSlot(InventoryItemSlot slot)
    {
        // If nothing's equipped, return
        if( !gear[slot] ){
            return;
        }

        // Remove your passive upgrades from that item
        RemoveStatBonusesFromItem(gear[slot].data);

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

        // Delete the thing
        Destroy(gear[slot].gameObject);

        // Set value to null in the dictionary and clear the UI
        ClearItemSlot(slot);
        AlertTextUI.instance.EnableItemExamineAlert();        
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

    public bool ItemSlotIsFull(InventoryItemSlot slot)
    {
        return gear[slot] != null;
    }

    // Called when you die or return to main menu
    public void ClearRunInventory()
    {
        if(weaponModel){
            RemoveEquippedWeaponModel();
        }

        // Delete each item object and clear the inventory slot
        // (Can't loop because can't delete the things while iterating, stuff gets mad)
        if(gear[InventoryItemSlot.Weapon]){
            Destroy(gear[InventoryItemSlot.Weapon].gameObject);
            ClearItemSlot(InventoryItemSlot.Weapon);
        }
        if(gear[InventoryItemSlot.Accessory]){
            Destroy(gear[InventoryItemSlot.Accessory].gameObject);
            ClearItemSlot(InventoryItemSlot.Accessory);
        }
        if(gear[InventoryItemSlot.Legs]){
            Destroy(gear[InventoryItemSlot.Legs].gameObject);
            ClearItemSlot(InventoryItemSlot.Legs);
        }
        if(gear[InventoryItemSlot.Helmet]){
            Destroy(gear[InventoryItemSlot.Helmet].gameObject);
            ClearItemSlot(InventoryItemSlot.Helmet);
        }

        SetTempCurrency(0);
        ClearHealthPotions();

        hasPickedSomethingUpThisRun = false;
    }

    public void SetTempCurrency(int value)
    {
        tempCurrency = value;
        InGameUIManager.instance.SetTempCurrencyValue(tempCurrency);
    }

    public void SetPermanentCurrency(int value, bool setUI = true)
    {
        permanentCurrency = value;

        if(setUI){
            InGameUIManager.instance.SetPermanentCurrencyValue(permanentCurrency);
        }        
    }

    public void SpendPermanentCurrency(int amountSpent)
    {
        permanentCurrency -= amountSpent;
        PermanentUpgradeManager.instance.totalPermanentCurrencySpent += amountSpent;
        InGameUIManager.instance.SetPermanentCurrencyValue(permanentCurrency);
    }

    public void ResetPermanentCurrency()
    {
        permanentCurrency += PermanentUpgradeManager.instance.totalPermanentCurrencySpent;
        PermanentUpgradeManager.instance.totalPermanentCurrencySpent = 0;
        InGameUIManager.instance.SetPermanentCurrencyValue(permanentCurrency);        
    }
}
