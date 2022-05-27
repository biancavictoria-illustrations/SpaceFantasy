using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomForceFields : MonoBehaviour
{
    public List<GameObject> forceFields = new List<GameObject>();

    // TODO: Activate force fields on generation complete?

    public void RoomOpenOnObjectInteracted()
    {
        foreach(GameObject ff in forceFields)
            ff.SetActive(false);
    }
}
