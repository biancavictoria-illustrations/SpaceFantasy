using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class of all types of items, including weapons
public class EquipmentBase : MonoBehaviour
{
    public EquipmentData equipmentData {get; private set;}

    // Generated when the item is created
    public ItemRarity rarity {get; private set;}
    public int enhancementCount {get; private set;}
    public EquipmentStats stats;

    // Dropped gear you are in range of
    public static EquipmentBase ActiveGearDrop {get; private set;}

    public void SetNewItemValues(EquipmentData _data, EquipmentStats _stats, ItemRarity _rarity, int _enhancements)
    {
        equipmentData = _data;
        stats = _stats;
        rarity = _rarity;
        enhancementCount = _enhancements;
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
