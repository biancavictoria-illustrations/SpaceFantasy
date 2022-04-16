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
            AlertTextUI.instance.EnableItemPickupAlert();

            Debug.Log("ITEM ACTIVE");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveGearDrop = null;
            AlertTextUI.instance.DisableAlert();

            Debug.Log("ITEM INACTIVE (left radius)");
        }
    }

    // Called in the EnemyDropGenerator script (as well as inventory when you remove something?)
    public void DropItemModelIn3DSpace()
    {
        // Instantiate the MODEL prefab as a child of THIS game object (the Drop Item Prefab)
        Instantiate(GetComponent<GeneratedEquipment>().equipmentBaseData.ItemDropModelPrefab(), transform);
    }
}
