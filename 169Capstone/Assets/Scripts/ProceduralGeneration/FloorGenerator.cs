using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorGenerator : MonoBehaviour
{
    private const int maxNumberOfPlacementAttempts = 50;

    [Header("Generation parameters")]
    [Tooltip("The size of one grid cell in Unity units.")]
    public float gridCellSizeInUnits = 20;

    [Tooltip("The size of the grid one which to generate the rooms.")]
    public Vector2Int gridBounds;
    [Tooltip("The ideal percentage of grid spaces that should be occupied by encounter rooms.")]
    public float encounterRoomRatio = 0.5f;
    [Tooltip("The closest two rooms can be together (measured in terms of grid spaces).")]
    public float encounterRoomBuffer = 1;
    [Tooltip("The maximum distance that two rooms can be and still get connected by a hallway (measured in terms of grid spaces).")]
    public float maxHallwayLength = 3;

    [Header("Asset References")]
    [Tooltip("The material to be applied to the force fields that trap players inside an encounter room.")]
    public Material forceFieldMat;

    public GameObject spawnRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject shopRoomPrefab;

    [Tooltip("A list of all room prefabs which will trap the player inside until they defeat all enemies.")]
    public List<GameObject> encounterRoomPrefabs;
    [Tooltip("A list of all room prefabs which will connect encounter rooms together.")]
    public List<GameObject> hallwayRoomPrefabs;


    //TODO delete this, only use this for testing
    void Awake()
    {
        Generate();
    }
    //TODO delete this, only use this for testing


    public void Configure(float? encounterRoomRatio = null, Vector2Int? gridBounds = null)
    {
        if(encounterRoomRatio.HasValue) 
            this.encounterRoomRatio = encounterRoomRatio.Value;
        if(gridBounds.HasValue) 
            this.gridBounds = gridBounds.Value;
    }

    public void Generate()
    {   
        //Declare a list of encounter rooms
        List<GameObject> encounterRooms = new List<GameObject>();

        #region Preliminary Room Placement

        //Place down the spawn, shops, and boss room first
        GameObject spawnRoom = Instantiate(spawnRoomPrefab, new Vector3(-0.5f, 0, -0.5f) * gridCellSizeInUnits, Quaternion.identity);

        GameObject centerShop = Instantiate(shopRoomPrefab, new Vector3(-0.5f - gridBounds.x / 2, 0, -0.5f - gridBounds.y / 2) * gridCellSizeInUnits, Quaternion.identity);
        GameObject xShop = Instantiate(shopRoomPrefab, new Vector3(-gridBounds.x + 0.5f, 0, -0.5f) * gridCellSizeInUnits, Quaternion.identity);
        GameObject zShop = Instantiate(shopRoomPrefab, new Vector3(-0.5f, 0, -gridBounds.y + 0.5f) * gridCellSizeInUnits, Quaternion.identity);

        GameObject bossRoom = Instantiate(bossRoomPrefab, new Vector3(-gridBounds.x + 0.5f, 0, -gridBounds.y + 0.5f) * gridCellSizeInUnits, Quaternion.identity);

        #endregion

        #region Encounter Room Placement

            //Place down all encounter rooms
            float targetSpacesFilled = gridBounds.x * gridBounds.y * encounterRoomRatio;
            int spacesFilled = 0;
            int tries = 0;

            //Stop if we reach our target amount or if we fail to place a room
            while(spacesFilled < targetSpacesFilled && tries < maxNumberOfPlacementAttempts)
            {
                tries = 0;
                while(tries < maxNumberOfPlacementAttempts) //Try a certain number of times to place a room down. If we can't do it in a predetermined number of tries, move on to placing hallways
                {
                    //Choose a random room, position, and rotation
                    GameObject prefab = encounterRoomPrefabs[Random.Range(0, encounterRoomPrefabs.Count)];
                    Vector3 position = new Vector3(-Random.Range(0, gridBounds.x) - 0.5f, 0, -Random.Range(0, gridBounds.y) - 0.5f) * gridCellSizeInUnits; //TODO limit based on the max dimensions of the room
                    Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90, Vector3.up);

                    //Instantiate the room at the chosen coordinates and grab its Room script and its bounds collider
                    GameObject currentRoom = Instantiate(prefab, position, rotation);
                    Room roomScript = currentRoom.GetComponentInChildren<Room>();
                    BoxCollider[] roomBoundsList = roomScript.GetComponents<BoxCollider>();

                    //If any of the colliders intersect another room, try again
                    bool intersects = false;
                    foreach(BoxCollider roomBounds in roomBoundsList)
                    {
                        //We use > 1 since we know the bounds will always overlap the collider we used as a reference
                        if(Physics.OverlapBox(roomBounds.transform.position + roomBounds.center, 
                                                roomBounds.bounds.extents + (Vector3.one * gridCellSizeInUnits * encounterRoomBuffer), 
                                                roomBounds.transform.rotation, 
                                                LayerMask.GetMask("RoomBounds")).Length > 1)
                        {
                            intersects = true;
                            break;
                        }
                    }

                    //If the room overlaps another room, destroy the room. 
                    //Since Unity doesn't like DestroyImmediate we'll disable the collider and pretend the room no longer exists
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
            if(tries == maxNumberOfPlacementAttempts)
                Debug.Log("Ran out of attempts to place a room");

        #endregion

        #region Hallway Placement

            //Find and store pairs of nearby rooms
            #region Find Connectible Rooms

                Dictionary<GameObject, HashSet<GameObject>> roomsWithinRange = new Dictionary<GameObject, HashSet<GameObject>>();

                //Add nearby rooms for encounter rooms
                foreach(GameObject room in encounterRooms)
                    foreach(GameObject destRoom in findRoomsWithinSphere(room.transform.position, maxHallwayLength))
                        AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, room, destRoom);

                //Add nearby rooms for non-encounter rooms
                foreach(GameObject destRoom in findRoomsWithinSphere(spawnRoom.transform.position, maxHallwayLength))
                    AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, spawnRoom, destRoom);

                foreach(GameObject destRoom in findRoomsWithinSphere(centerShop.transform.position, maxHallwayLength))
                    AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, centerShop, destRoom);

                foreach(GameObject destRoom in findRoomsWithinSphere(xShop.transform.position, maxHallwayLength))
                    AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, xShop, destRoom);

                foreach(GameObject destRoom in findRoomsWithinSphere(zShop.transform.position, maxHallwayLength))
                    AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, zShop, destRoom);

                foreach(GameObject destRoom in findRoomsWithinSphere(bossRoom.transform.position, maxHallwayLength))
                    AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, bossRoom, destRoom);

            #endregion

            //Define a couple useful functions
            #region Generation Helpers

                bool CreateHallwayBetweenRooms(GameObject startRoom, GameObject destRoom)
                {
                    List<GameObject> hallwayPrefabs = new List<GameObject>(hallwayRoomPrefabs);
                    bool success = false;

                    do
                    {
                        //Pick a random hallway piece
                        int index = Random.Range(0, hallwayPrefabs.Count);
                        GameObject prefab = hallwayPrefabs[index];
                        hallwayPrefabs.RemoveAt(index);

                        //
                    }
                    while(!success && hallwayPrefabs.Count > 0);

                    return false;
                }

                bool CreateChainOfHallwaysBetweenRooms(GameObject startRoom, GameObject destRoom)
                {
                    if(roomsWithinRange[startRoom].Contains(destRoom))
                    {
                        //If startRoom and destRoom can be connected by a hallway, try to connect them
                        return CreateHallwayBetweenRooms(startRoom, destRoom);
                    }
                    else
                    {
                        //For each room we can connect startRoom to, see if we can create a chain that leads to destRoom
                        HashSet<GameObject> validRooms = roomsWithinRange[startRoom];
                        GameObject nextClosestRoom;
                        bool connectionSuccessful = false;

                        do
                        {
                            //Pick an arbitrary room from the set of valid rooms
                            nextClosestRoom = validRooms.ElementAt(0);
                            float shortestDistance = Vector3.Distance(nextClosestRoom.transform.position, destRoom.transform.position);

                            //Find the closest valid room to destRoom
                            foreach(GameObject room in validRooms)
                            {
                                float distance = Vector3.Distance(room.transform.position, destRoom.transform.position);
                                if(distance < shortestDistance)
                                {
                                    nextClosestRoom = room;
                                    shortestDistance = distance;
                                }
                            }

                            //Attempt to create a chain between this room and destRoom
                            connectionSuccessful = CreateChainOfHallwaysBetweenRooms(nextClosestRoom, destRoom);
                            //Whether it succeeds or not, we want to remove the rooms from each other's sets of valid rooms so we know not to try to connect them again later
                            RemoveKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, startRoom, nextClosestRoom);
                        } 
                        while(!connectionSuccessful && validRooms.Count > 0);

                        if(connectionSuccessful)
                            connectionSuccessful = CreateHallwayBetweenRooms(startRoom, nextClosestRoom);
                        
                        return connectionSuccessful;
                    }
                }

            #endregion
            
            //Start by ensuring there's one guaranteed path from spawn to center shop, and then from center shop to edge shops and boss room
            #region Necessary Hallway Generation

                bool success;
                success = CreateChainOfHallwaysBetweenRooms(spawnRoom, centerShop);
                if(!success)
                {
                    Debug.LogError("Procedural Generation Failed: Could not create a path from spawn room to center shop.");
                }

                success = CreateChainOfHallwaysBetweenRooms(centerShop, xShop);
                if(!success)
                {
                    Debug.LogError("Procedural Generation Failed: Could not create a path from center shop to secondary shop.");
                }

                success = CreateChainOfHallwaysBetweenRooms(centerShop, zShop);
                if(!success)
                {
                    Debug.LogError("Procedural Generation Failed: Could not create a path from center shop to secondary shop.");
                }

                success = CreateChainOfHallwaysBetweenRooms(centerShop, bossRoom);
                if(!success)
                {
                    Debug.LogError("Procedural Generation Failed: Could not create a path from center shop to boss room.");
                }

            #endregion

            //Iterate through pairs of nearby rooms and connect them with hallways

        #endregion
    }

    private HashSet<GameObject> findRoomsWithinSphere(Vector3 center, float radius, bool increaseRadiusIfNoneFound = true)
    {
        //Using a hashset guarantees there will be no duplicate rooms
        HashSet<GameObject> rooms = new HashSet<GameObject>();
        do
        {
            foreach(Collider col in Physics.OverlapSphere(center, radius * gridCellSizeInUnits, LayerMask.GetMask("RoomBounds")))
                rooms.Add(col.gameObject);
            
            ++radius;
        } while(increaseRadiusIfNoneFound && rooms.Count == 0);
        
        return rooms;
    }

    private void AddKeyValuePairReflexively<T, C>(Dictionary<T, C> dictionary, T key, T value) where C : ICollection<T>, new()
    {
        if(!dictionary.ContainsKey(key))
            dictionary.Add(key, new C());

        if(!dictionary.ContainsKey(value))
            dictionary.Add(value, new C());
        
        dictionary[key].Add(value);
        dictionary[value].Add(key);
    }

    private void RemoveKeyValuePairReflexively<T, C>(Dictionary<T, C> dictionary, T key, T value) where C : ICollection<T>
    {
        if(dictionary.ContainsKey(key))
            dictionary[key].Remove(value);

        if(!dictionary.ContainsKey(value))
            dictionary[value].Remove(key);
    }
}
