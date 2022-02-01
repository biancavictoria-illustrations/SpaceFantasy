using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentBase : MonoBehaviour
{
    // Data all equipment shares
    public EquipmentData equipmentData {get; private set;}

    // Generated when the item is created
    public ItemRarity rarity {get; private set;}
    public int baseCost {get; private set;}
    public int enhancementCount {get; private set;}

    // Dropped gear you are in range of
    public static EquipmentBase ActiveGearDrop {get; private set;}


    public void GenerateItemValues(EquipmentData data, ItemRarity itemRarity = ItemRarity.Common, int itemCost = 0, int enhancements = 0)
    {
        equipmentData = data;

        rarity = itemRarity;
        baseCost = itemCost;
        enhancementCount = enhancements;

        // TODO: Generate the values if no values are provided
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveGearDrop = this;
            AlertTextUI.instance.EnableItemPickupAlert();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveGearDrop = null;
            AlertTextUI.instance.DisableAlert();
        }
    }
}
