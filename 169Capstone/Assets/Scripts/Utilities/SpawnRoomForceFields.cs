using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomForceFields : MonoBehaviour
{
    public List<GameObject> forceFields = new List<GameObject>();

    [Tooltip("List of options of objects for the player to interact with in order to drop the force fields; for normal spawn room, should contain one of each weapon; for run 1 spawn room, the Captain's Log object")]
    public List<GameObject> interactableObjectsToOpenRoom = new List<GameObject>();

    // TODO: Activate force fields on generation complete

    public void RoomOpenOnObjectInteracted()
    {
        foreach(GameObject ff in forceFields)
            ff.SetActive(false);
    }
}
