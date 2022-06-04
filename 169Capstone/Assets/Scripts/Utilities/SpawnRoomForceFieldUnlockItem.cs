using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomForceFieldUnlockItem : MonoBehaviour
{
    public static SpawnRoomForceFieldUnlockItem activeForceFieldUnlockItem {get; private set;}

    public SpawnRoomForceFields spawnRoomForceFields;

    [Tooltip("Only false for Captain's Log")]
    public bool isWeapon = true;

    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            activeForceFieldUnlockItem = this;

            if(!isWeapon)   // If a weapon, alert is handled there
                AlertTextUI.instance.EnablePickupAlert();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            activeForceFieldUnlockItem = null;

            if(!isWeapon)   // If a weapon, alert is handled there
                AlertTextUI.instance.DisablePrimaryAlert();
        }
    }

    public void UnlockForceFieldsOnPickUp()
    {
        activeForceFieldUnlockItem = null;
        
        // Start the game timer
        GameManager.instance.gameTimer.timerHasStartedForRun = true;
        InputManager.instance.RunGameTimer(true,true);

        if(spawnRoomForceFields)
            spawnRoomForceFields.RoomOpenOnObjectInteracted();
        else
            Debug.LogError("No spawn room force fields found! Cannot drop force fields");

        if(!isWeapon){
            // If a weapon, alert is handled there
            AlertTextUI.instance.DisablePrimaryAlert();
            PlayerInventory.hasCaptainsLog = true;
        }

        Destroy(gameObject);
    }
}
