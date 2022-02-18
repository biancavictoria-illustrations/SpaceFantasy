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

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }

        gear = new Dictionary<InventoryItemSlot, Equipment>();
    }

    void Start()
    {
        // Set default values for all slots
        gear[InventoryItemSlot.Weapon] = null;
        gear[InventoryItemSlot.Accessory] = null;
        gear[InventoryItemSlot.Helmet] = null;
        gear[InventoryItemSlot.Boots] = null;
    }

    public void UseHealthPotion()
    {
        if(healthPotionQuantity == 0){
            // TODO: probably give UI feedback, like a little "no potions!" pop up
            return;
        }

        EntityHealth playerHealth = Player.instance.GetComponent<EntityHealth>();
        float healedHitPoints = playerHealth.maxHitpoints * (0.01f * Player.instance.GetComponent<PlayerStats>().getHealingEfficacy());
        playerHealth.Heal(healedHitPoints);

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

    public void EquipItem(InventoryItemSlot slot, Equipment item)
    {
        // If necessary, drop the PREVIOUS item on the ground
        if( gear[slot] ){
            GameObject dropItem = Instantiate(dropItemPrefab, Player.instance.transform.position, Quaternion.identity);
            dropItem.GetComponent<GeneratedEquipment>().SetAllEquipmentData(gear[slot].data);
            dropItem.GetComponent<DropTrigger>().DropItemModelIn3DSpace();
            
            if(slot == InventoryItemSlot.Weapon){
                RemoveEquippedWeaponModel();
            }
        }
        gear[slot] = item;  // Set the slot to now be the NEW item
        InGameUIManager.instance.SetGearItemUI(slot, item.data.equipmentBaseData.Icon());   // Update UI accordingly
    }

    private void RemoveEquippedWeaponModel()
    {
        Destroy(weaponModel);
    }

    public void ClearItemSlot(InventoryItemSlot slot)
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
        foreach( KeyValuePair<InventoryItemSlot,Equipment> item in gear ){
            if(item.Value){
                Destroy(item.Value);
                ClearItemSlot(item.Key);
            }
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
}
