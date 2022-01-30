using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: set these to whatever we're actually calling them in game
public enum InventoryItemSlot
{
    Helmet,
    Accessory,
    Boots,
    Weapon,
    Potion,

    enumSize
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    // gear dictionary contains keys of the enum item slot value and values of the actual item
    public Dictionary<InventoryItemSlot,GameObject> gear = new Dictionary<InventoryItemSlot, GameObject>();

    public int healthPotionQuantity {get; private set;}

    public int tempCurrency;
    public int permanentCurrency;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    public void UseHealthPotion()
    {
        if(healthPotionQuantity > 0){
            // TODO: Update health
            healthPotionQuantity--;
        }
    }

    public void PurchaseHealthPotion()
    {
        healthPotionQuantity++;
    }

    public void EquipItem(InventoryItemSlot slot, GameObject item)
    {
        gear[slot] = item;
        // TODO: Call the update on the inventory UI
    }
}
