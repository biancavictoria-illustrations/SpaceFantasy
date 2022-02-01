using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentBase : MonoBehaviour
{
    // Data all equipment shares
    public EquipmentData equipmentData;

    // Generated when the item is created
    public ItemRarity rarity;
    public int baseCost;    
    public int enhancementCount;

    // Dropped gear you are in range of
    public static EquipmentBase ActiveGearDrop {get; private set;}


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
