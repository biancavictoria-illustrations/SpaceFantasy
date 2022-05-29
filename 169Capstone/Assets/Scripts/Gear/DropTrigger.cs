using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stored on Drop Item prefab to watch the trigger
public class DropTrigger : MonoBehaviour
{
    // Dropped gear you are in range of (goes in spawned equipment game object)
    public static GeneratedEquipment ActiveGearDrop {get; private set;}

    void Start()
    {
        ActiveGearDrop = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveGearDrop = GetComponent<GeneratedEquipment>();
            AlertTextUI.instance.EnableItemExamineAlert();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveGearDrop = null;
            AlertTextUI.instance.DisablePrimaryAlert();
        }
    }

    // Called in the EnemyDropGenerator script (as well as inventory when you remove something?)
    public void DropItemModelIn3DSpace()
    {
        GeneratedEquipment generatedEquipment = GetComponent<GeneratedEquipment>();

        // Instantiate the MODEL prefab as a child of THIS game object (the Drop Item Prefab)
        GameObject itemDrop = Instantiate(generatedEquipment.equipmentBaseData.ItemDropModelPrefab(), transform);

        // Get FX that coordinates to type + rarity
        foreach( GearManagerObject.ItemFXData fxData in GameManager.instance.GearManager().ItemFXDataList() ){
            // Check if this item is a weapon or non-weapon gear
            bool itemIsWeapon = generatedEquipment.equipmentBaseData.ItemSlot() == InventoryItemSlot.Weapon;

            // If rarity and type matches, instantiate this FX
            if( fxData.Rarity() == generatedEquipment.rarity && itemIsWeapon == fxData.ItemIsWeapon() ){
                // Instantiate and return
                GameObject fx = Instantiate( fxData.FXPrefab(), itemDrop.transform.position, Quaternion.identity );
                fx.transform.parent = itemDrop.transform;
                // Instantiate( fxData.FXPrefab(), Vector3.zero, Quaternion.identity, itemDrop.transform );
                return;
            }
        }
    }
}
