using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorGenerator : MonoBehaviour
{
    public float gridCellSizeInUnits = 20;

    public float encounterRoomRatio = 0.5f;
    public Vector2Int gridBounds;

    public GameObject spawnRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject shopRoomPrefab;
    public List<GameObject> encounterRoomPrefabs;
    public List<GameObject> hallwayRoomPrefabs;

    void Awake()
    {
        //TODO delete this, only use this for testing

        Generate();
    }

    public void Configure(float? encounterRoomRatio = null, Vector2Int? gridBounds = null)
    {
        if(encounterRoomRatio.HasValue) 
            this.encounterRoomRatio = encounterRoomRatio.Value;
        if(gridBounds.HasValue) 
            this.gridBounds = gridBounds.Value;
    }

    public void Generate()
    {
        //Place down the spawn, shops, and boss room first
        Instantiate(spawnRoomPrefab, new Vector3(0.5f, 0, 0.5f) * gridCellSizeInUnits, Quaternion.identity);

        Instantiate(shopRoomPrefab, new Vector3(gridBounds.x - 0.5f, 0, 0.5f) * gridCellSizeInUnits, Quaternion.identity);
        Instantiate(shopRoomPrefab, new Vector3(0.5f + gridBounds.x / 2, 0, 0.5f + gridBounds.y / 2) * gridCellSizeInUnits, Quaternion.identity);
        Instantiate(shopRoomPrefab, new Vector3(0.5f, 0, gridBounds.y - 0.5f) * gridCellSizeInUnits, Quaternion.identity);

        Instantiate(bossRoomPrefab, new Vector3(gridBounds.x - 0.5f, 0, gridBounds.y - 0.5f) * gridCellSizeInUnits, Quaternion.identity);


        //Place down all encounter rooms
        List<GameObject> encounterRooms = new List<GameObject>();

        float targetSpacesFilled = gridBounds.x * gridBounds.y * encounterRoomRatio;
        int spacesFilled = 0;
        int tries = 0;

        //Stop if we reach our target amount or if we fail to place a room
        while(spacesFilled < targetSpacesFilled && tries < 50)
        {
            tries = 0;
            while(tries < 50) //Try 50 times to place a room down. If we can't do it in 50 tries, move on to hallways
            {
                //Choose a random room, position, and rotation
                GameObject prefab = encounterRoomPrefabs[Random.Range(0, encounterRoomPrefabs.Count)];
                Vector3 position = new Vector3(Random.Range(0, gridBounds.x) + 0.5f, 0, Random.Range(0, gridBounds.y) + 0.5f) * gridCellSizeInUnits; //TODO limit based on the max dimensions of the room
                Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90, Vector3.up);

                //Instantiate the room at the chosen coordinates and grab its Room script and its bounds collider
                GameObject currentRoom = Instantiate(prefab, position, rotation);
                Room roomScript = currentRoom.GetComponentInChildren<Room>();
                BoxCollider[] roomBoundsList = roomScript.GetComponents<BoxCollider>();

                //If any of the colliders intersect another room, try again
                bool intersects = false;
                foreach(BoxCollider roomBounds in roomBoundsList)
                {
                    if(Physics.OverlapBox(roomBounds.transform.position + roomBounds.center, roomBounds.bounds.extents, roomBounds.transform.rotation, LayerMask.GetMask("RoomBounds")).Length > 1)
                    {
                        intersects = true;
                        break;
                    }
                }

                //If the room overlaps another room, destroy the room. 
                //Since Unity doesn't like DestroyImmediate we'll disable the collider to simulate the room no longer existing
                //We use > 1 since we know the bounds will always overlap the collider we used as a reference
                if(intersects)
                {
                    foreach(BoxCollider roomBounds in roomBoundsList)
                    {
                        roomBounds.enabled = false;
                    }

                    Destroy(currentRoom);
                    ++tries;

                    Debug.Log("Room intersects existing room, attempt number " + tries);
                    continue;
                }
                else //We successfully placed a room down. Add it to the list and update our progress
                {
                    spacesFilled += 3; //TODO Replace this with the number of grid spaces the room takes up (should be in the Room script)
                    encounterRooms.Add(currentRoom);
                    break;
                }
            }
        }

        if(tries == 50)
            Debug.Log("Ran out of attempts to place a room");

        //Start by ensuring there's one guaranteed path from spawn to shop, and then from shop to boss room

        //Find pairs of nearby encounter rooms and connect them with hallways
    }
}
