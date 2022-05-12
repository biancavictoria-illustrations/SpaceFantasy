using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Events;

public class FloorGenerator : MonoBehaviour
{
    public class OnGenerationCompleteEvent : UnityEvent {};

    private const int maxNumberOfPlacementAttempts = 100;
    
    public delegate void CallbackBoolDelegate(bool success);
    private delegate void CallbackDelegate();

    [Tooltip("Whether to generate the floor in the script's Start function.")]
    public bool generateOnStart = false;

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
    [Tooltip("The prefab for the room that the player starts in.")]
    public GameObject spawnRoomPrefab;
    [Tooltip("The prefab for the beetle boss fight.")]
    public GameObject bossRoomPrefab;
    [Tooltip("The prefab for the three shops.")]
    public GameObject shopRoomPrefab;
    [Tooltip("The prefab for the force fields that trap players inside an encounter room.")]
    public GameObject forceFieldPrefab;
    [Tooltip("The prefab that contains a navmesh and the necessary settings for combat.")]
    public GameObject navMeshPrefab;
    [Tooltip("The player prefab.")]
    public GameObject playerPrefab;

    [Tooltip("A list of all room prefabs which will trap the player inside until they defeat all enemies.")]
    public List<GameObject> encounterRoomPrefabs;
    [Tooltip("A list of all room prefabs which will connect encounter rooms together.")]
    public List<GameObject> hallwayRoomPrefabs;

    public OnGenerationCompleteEvent OnGenerationComplete;

    void Awake()
    {
        OnGenerationComplete = new OnGenerationCompleteEvent();

        OnGenerationComplete.AddListener( () => {
            Transform spawnPoint = GameObject.Find("SpawnPoint").transform;
            Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        } );
    }

    void Start()
    {
        if(generateOnStart)
        {
            StartCoroutine(GenerateUntilSuccessful());
        }
    }

    public void Configure(float? encounterRoomRatio = null, Vector2Int? gridBounds = null)
    {
        if(encounterRoomRatio.HasValue) 
            this.encounterRoomRatio = encounterRoomRatio.Value;
        if(gridBounds.HasValue) 
            this.gridBounds = gridBounds.Value;
    }

    public void Generate(CallbackBoolDelegate successCallback)
    {   
        //Declare a list of encounter rooms
        List<GameObject> encounterRooms = new List<GameObject>();
        List<GameObject> hallwayRooms = new List<GameObject>();

#region Preliminary Room Placement

        //Place down the spawn, shops, and boss room first
        GameObject spawnRoom = Instantiate(spawnRoomPrefab, new Vector3(-0.5f, 0, -0.5f) * gridCellSizeInUnits, Quaternion.identity);

        GameObject centerShop = Instantiate(shopRoomPrefab, new Vector3(-0.5f - gridBounds.x / 2, 0, -0.5f - gridBounds.y / 2) * gridCellSizeInUnits, Quaternion.identity);
        GameObject xShop = Instantiate(shopRoomPrefab, new Vector3(-gridBounds.x + 0.5f, 0, -0.5f) * gridCellSizeInUnits, Quaternion.identity);
        GameObject zShop = Instantiate(shopRoomPrefab, new Vector3(-0.5f, 0, -gridBounds.y + 0.5f) * gridCellSizeInUnits, Quaternion.identity);

        GameObject bossRoom = Instantiate(bossRoomPrefab, new Vector3(-gridBounds.x + 0.5f, 0, -gridBounds.y + 0.5f) * gridCellSizeInUnits, Quaternion.identity);
        Beetle beetleBoss = bossRoom.GetComponentInChildren<Beetle>();
        beetleBoss.gameObject.SetActive(false);

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

                //Instantiate the room at the chosen coordinates and grab its Room script and its bounds colliders
                GameObject currentRoom = Instantiate(prefab, position, rotation);
                Room roomScript = currentRoom.GetComponentInChildren<Room>();
                List<BoxCollider> roomBoundsList = new List<BoxCollider>(roomScript.GetComponents<BoxCollider>());

                //If any of the colliders intersect another room, try again
                bool intersects = false;
                foreach(BoxCollider roomBounds in roomBoundsList)
                {
                    foreach(BoxCollider room in Physics.OverlapBox(roomBounds.transform.position + roomBounds.center, 
                                                                    roomBounds.bounds.extents + (Vector3.one * gridCellSizeInUnits * encounterRoomBuffer), 
                                                                    roomBounds.transform.rotation, 
                                                                    LayerMask.GetMask("RoomBounds")))
                    {
                        //If the box overlaps a collider that doesn't belong to this room, it intersects something else
                        if(!roomBoundsList.Contains(room))
                        {
                            intersects = true;
                            break;
                        }
                    }

                    if(intersects)
                        break;
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

                    // Debug.Log("Room intersects existing room, attempt number " + tries);
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
        // if(tries == maxNumberOfPlacementAttempts)
        //     Debug.Log("Ran out of attempts to place a room");

#endregion

#region Hallway Placement

    #region Find Connectible Rooms
        //Find and store pairs of nearby rooms
        Dictionary<GameObject, HashSet<GameObject>> roomsWithinRange = new Dictionary<GameObject, HashSet<GameObject>>();
        float maxSearchRadius = gridBounds.magnitude;

        //Add nearby rooms for non-encounter rooms
        foreach(GameObject destRoom in FindNewRoomsWithinSphere(roomsWithinRange, spawnRoom, maxHallwayLength, maxHallwayLength * 2))
            AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, spawnRoom, destRoom);

        foreach(GameObject destRoom in FindNewRoomsWithinSphere(roomsWithinRange, centerShop, maxHallwayLength, maxHallwayLength * 2))
            AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, centerShop, destRoom);

        foreach(GameObject destRoom in FindNewRoomsWithinSphere(roomsWithinRange, xShop, maxHallwayLength, maxHallwayLength * 2))
            AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, xShop, destRoom);

        foreach(GameObject destRoom in FindNewRoomsWithinSphere(roomsWithinRange, zShop, maxHallwayLength, maxHallwayLength * 2))
            AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, zShop, destRoom);

        foreach(GameObject destRoom in FindNewRoomsWithinSphere(roomsWithinRange, bossRoom, maxHallwayLength, maxHallwayLength * 2))
            AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, bossRoom, destRoom);

        //Add nearby rooms for encounter rooms
        foreach(GameObject room in encounterRooms)
        {
            foreach(GameObject destRoom in FindNewRoomsWithinSphere(roomsWithinRange, room, maxHallwayLength, maxHallwayLength * 2))
                AddKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, room, destRoom);
        }

    #endregion

    #region Hallway Generation Routines

        IEnumerator GenerateAllHallwaysRoutine()
        {
            bool returnValue = false;

            IEnumerator CreateHallwayBetweenRoomsRoutine(GameObject startRoom, GameObject destRoom, float previousDistance, CallbackDelegate callback)
            {
                //Failsafe to prevent too many attempts to connect
                bool hasFailed = false;

                //Get the list of active exits on startRoom
                List<Transform> startRoomExits = new List<Transform>(startRoom.GetComponentInChildren<Room>().roomExits.Where((Transform t) => t.gameObject.activeSelf));

                //If there are no active exits on startRoom, a connection is impossible
                if(startRoomExits.Count == 0) 
                {
                    Debug.LogWarning("No exits on startRoom");
                }

                //Try to connect to destRoom with each of startRoom's exits, in order of which exit is closest
                while(startRoomExits.Count > 0)
                {
                    //Get a list of all active exits on destRoom
                    List<Transform> destRoomExits = new List<Transform>(destRoom.GetComponentInChildren<Room>().roomExits.Where((Transform t) => t.gameObject.activeSelf));

                    //If there are no active exits on destRoom, a connection is impossible
                    if(destRoomExits.Count == 0) 
                    {
                        Debug.LogWarning("No exits on destRoom");
                        break;
                    }

                    //Find the closest pair of exits and start with those
                    var transformPair = FindClosestTransformPair(startRoomExits, destRoomExits, Vector3.up, 5);
                    if(transformPair.Key == null || transformPair.Value == null) //If the result is invalid for some reason, abort
                    {
                        Debug.LogWarning("No valid exit pairs");
                        break;
                    }
                    
                    //Assign exit values
                    Transform startExit = transformPair.Key;
                    Transform destExit = null;

                    //Remove startExit from the list of options
                    startRoomExits.Remove(startExit);

                    //Try to connect to startExit with each of destRoom's exits, in order of which exit is closest
                    while(destRoomExits.Count > 0)
                    {
                        if(destExit == null)
                        {
                            destExit = transformPair.Value;
                        }
                        else
                        {
                            destExit = FindClosestTransformToDestination(destRoomExits, startExit, Vector3.up, 5);
                        }
                        destRoomExits.Remove(destExit);
                        
                        float distanceBetweenStartAndDest = Vector3.Distance(startExit.position, destExit.position);

                        // Debug.LogWarning("Picked Exits: " + startExit + " and " + destExit);
                        // Debug.LogWarning("startExit active: " + startExit.gameObject.activeSelf);
                        // Debug.LogWarning("destExit active: " + destExit.gameObject.activeSelf);
                        // Debug.LogWarning("Distance between exits: " + distanceBetweenStartAndDest);

                        //Keep picking hallway pieces until we find one that connects to destExit
                        List<GameObject> hallwayPrefabs = new List<GameObject>(hallwayRoomPrefabs);
                        int startingListSize = hallwayPrefabs.Count;
                        while(hallwayPrefabs.Count > startingListSize/2)
                        {
                            //Pick and instantiate a random hallway piece
                            int index = Random.Range(0, hallwayPrefabs.Count);
                            GameObject prefab = hallwayPrefabs[index];
                            hallwayPrefabs.RemoveAt(index);
                            GameObject hallwayPiece = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                            List<BoxCollider> roomBoundsList = new List<BoxCollider>();

                            //Try each orientation of the hallway until we find one that gets us closer to destExit
                            List<Transform> hallwayExits = hallwayPiece.GetComponentInChildren<Room>().roomExits;
                            foreach(Transform hallwayStartExit in hallwayExits)
                            {
                                //If the exit is already taken, log an error cause that shouldn't happen
                                if(!hallwayStartExit.gameObject.activeSelf)
                                {
                                    Debug.LogError("Hallway piece had disabled exit, this shouldn't be possible.");
                                    continue;
                                }

                                //Rotate the hallway to match startExit
                                Quaternion orientation = Quaternion.FromToRotation(hallwayStartExit.up, -startExit.up);
                                hallwayPiece.transform.rotation = orientation * hallwayPiece.transform.rotation;

                                //Make sure the hallway piece is oriented upright
                                if(hallwayPiece.transform.up != Vector3.up)
                                {
                                    hallwayPiece.transform.rotation = Quaternion.AngleAxis(180, hallwayStartExit.up) * hallwayPiece.transform.rotation;
                                }

                                //Move the hallway so exit matches startExit
                                Vector3 exitToCenter = hallwayPiece.transform.position - hallwayStartExit.position;
                                hallwayPiece.transform.position = startExit.position + exitToCenter;

                                //Check if the hallway piece overlaps another room
                                Room roomScript = hallwayPiece.GetComponentInChildren<Room>();
                                roomBoundsList = new List<BoxCollider>(roomScript.GetComponents<BoxCollider>());
                                Room intersectingRoom = null;
                                foreach(BoxCollider roomBounds in roomBoundsList)
                                {
                                    foreach(BoxCollider room in Physics.OverlapBox(roomBounds.transform.position + roomBounds.center, 
                                                                                    roomBounds.bounds.extents - (Vector3.one * 0.1f), 
                                                                                    roomBounds.transform.rotation, 
                                                                                    LayerMask.GetMask("RoomBounds")))
                                    {
                                        //If the box overlaps a collider that doesn't belong to this room, it intersects something else
                                        if(!roomBoundsList.Contains(room))
                                        {
                                            intersectingRoom = room.GetComponent<Room>();
                                            if(intersectingRoom != null)
                                            {
                                                // Debug.LogWarning(hallwayPiece + " intersects " + room.transform.parent.gameObject);
                                                break;
                                            }
                                            else
                                            {
                                                // Debug.LogWarning(hallwayPiece + " intersects " + room.transform.parent.gameObject + " but " + room.transform.parent.gameObject + " has no room script!?");
                                            }
                                        }
                                    }

                                    if(intersectingRoom != null)
                                        break;
                                }

                                //If the room overlaps another room, try to connect to it. If that fails, try a different orientation. 
                                //If we run out of possible orientations the algorithm will move on to a new hallway prefab
                                if(intersectingRoom != null)
                                {
                                    //Get the list of active exits on the intersecting room and try to connect to one
                                    List<Transform> intersectExits = new List<Transform>(intersectingRoom.roomExits.Where((Transform t) => t.gameObject.activeSelf));
                                    Transform exit = FindClosestTransformToDestination(intersectExits, startExit);

                                    //If the two exits are touching, connect them and consider this hallway a success
                                    if(exit != null && Vector3.Distance(startExit.position, exit.position) < 0.01f)
                                    {
                                        //Remove the walls between startRoom and intersectingRoom
                                        startExit.gameObject.SetActive(false);
                                        exit.gameObject.SetActive(false);

                                        //Disable the colliders and destroy the hallway piece since we didn't need it
                                        foreach(BoxCollider roomBounds in roomBoundsList)
                                        {
                                            roomBounds.enabled = false;
                                        }
                                        Destroy(hallwayPiece);

                                        // Debug.LogWarning("Hallway succeeded early");

                                        //COROUTINE STUFF
                                        yield return null;
                                        returnValue = true;
                                        callback();
                                        yield break;
                                        //COROUTINE STUFF

                                        //return true;
                                    }

                                    //Otherwise, try a new orientation of the same hallwayPiece
                                    // Debug.LogWarning("Intersected a room, trying new orientation.");
                                    continue;
                                }

                                //Find the closest active exit from hallwayPiece to destExit that isn't hallwayStartExit
                                List<Transform> otherExits = new List<Transform>(hallwayExits.Where((Transform t) => t.gameObject.activeSelf && t != hallwayStartExit));
                                Transform hallwayDestExit = FindClosestTransformToDestination(otherExits, destExit, Vector3.up, 5);

                                if(hallwayDestExit == null) 
                                {
                                    // Debug.LogWarning("Couldn't find a valid second exit on hallwayPiece " + hallwayPiece);
                                    continue;
                                }

                                float distanceFromHallwayToDest = Vector3.Distance(hallwayDestExit.position, destRoom.transform.position);

                                //Check if there is a different combination of exits that causes hallwayPiece to connect to destRoom
                                List<Transform> destRoomExitsPlusCurrentDestExit = new List<Transform>(destRoomExits);
                                destRoomExitsPlusCurrentDestExit.Add(destExit);
                                var kvp = FindClosestTransformPair(otherExits, destRoomExitsPlusCurrentDestExit, Vector3.up, 5);

                                //If there is a combination of exits that completes the hallway, choose those instead
                                if(Vector3.Distance(kvp.Key.position, kvp.Value.position) <= 0.1f)
                                {
                                    // Debug.LogWarning("Previous hallwayDistance: " + distanceFromHallwayToDest);
                                    // Debug.LogWarning("Previous hallwayDestExit: " + hallwayDestExit);
                                    // Debug.LogWarning("Previous destExit: " + destExit);

                                    hallwayDestExit = kvp.Key;
                                    destExit = kvp.Value;
                                    distanceFromHallwayToDest = Vector3.Distance(hallwayDestExit.position, destExit.position);

                                    // Debug.LogWarning("New hallwayDistance: " + distanceFromHallwayToDest);
                                    // Debug.LogWarning("New hallwayDestExit: " + hallwayDestExit);
                                    // Debug.LogWarning("New destExit: " + destExit);
                                }

                                //If the distance between hallwayPiece and destRoom is negligible we finally created a working hallway
                                if(distanceFromHallwayToDest < 0.1f)
                                {
                                    //Remove the walls between startRoom and hallwayPiece, as well as between hallwayPiece and destRoom
                                    startExit.gameObject.SetActive(false);
                                    hallwayStartExit.gameObject.SetActive(false);
                                    hallwayDestExit.gameObject.SetActive(false);
                                    destExit.gameObject.SetActive(false);

                                    // Debug.LogWarning("Successful Hallway");

                                    //Add the hallway to the list of all hallways
                                    hallwayRooms.Add(hallwayPiece);

                                    //COROUTINE STUFF
                                    yield return null;
                                    returnValue = true;
                                    callback();
                                    yield break;
                                    //COROUTINE STUFF

                                    //return true;
                                }
                                else if(distanceFromHallwayToDest < previousDistance) //Only continue the hallway if hallwayExit is closer to destExit than closestExit
                                {
                                    //Remove the walls between startRoom and hallwayPiece
                                    hallwayStartExit.gameObject.SetActive(false);
                                    startExit.gameObject.SetActive(false);
                                    // Debug.LogWarning("Case distanceFromHallwayToDest < distanceBetweenStartAndDest. Hallway distance = " + distanceFromHallwayToDest);

                                    //COROUTINE STUFF
                                    bool isRunning = true;
                                    yield return null;

                                    StartCoroutine(CreateHallwayBetweenRoomsRoutine(hallwayPiece, destRoom, distanceFromHallwayToDest, () => {isRunning = false;}));
                                    yield return new WaitWhile(() => isRunning);

                                    bool successfulHallway = returnValue;
                                    //COROUTINE STUFF

                                    //bool successfulHallway = CreateHallwayBetweenRooms(hallwayPiece, destRoom);
                                    // Debug.LogWarning("Recursive call was successful: " + successfulHallway);

                                    //If the hallway was successful, return true
                                    if(successfulHallway)
                                    {
                                        //Add the hallway to the list of all hallways
                                        hallwayRooms.Add(hallwayPiece);

                                        //COROUTINE STUFF
                                        returnValue = true;
                                        callback();
                                        yield break;
                                        //COROUTINE STUFF

                                        //return true;
                                    }
                                    else
                                    {
                                        //The hallway was unsuccessful, so we re-enable the walls between startRoom and hallwayPiece
                                        hallwayStartExit.gameObject.SetActive(true);
                                        startExit.gameObject.SetActive(true);

                                        //If this is the second recursive failure, return unsuccessful
                                        if(hasFailed)
                                        {
                                            //Disable the colliders and destroy the hallway piece
                                            foreach(BoxCollider roomBounds in roomBoundsList)
                                            {
                                                roomBounds.enabled = false;
                                            }
                                            Destroy(hallwayPiece);

                                            // Debug.LogWarning("Recursive call failed twice, cut off early");

                                            //COROUTINE STUFF
                                            yield return null;
                                            returnValue = false;
                                            callback();
                                            //COROUTINE STUFF
                                            
                                            //return false;
                                            
                                            yield break;
                                        }
                                        else
                                        {
                                            hasFailed = true;
                                        }
                                    }
                                }
                            }
                            
                            //This code is only reachable if this hallway piece was unsuccessful
                            //Disable the colliders and destroy the hallway piece
                            foreach(BoxCollider roomBounds in roomBoundsList)
                            {
                                roomBounds.enabled = false;
                            }
                            // Debug.LogWarning("Could not place hallway " + hallwayPiece + " connected to " + startRoom);
                            Destroy(hallwayPiece);
                        }
                    }
                }

                //COROUTINE STUFF
                yield return null;
                returnValue = false;
                callback();
                //COROUTINE STUFF

                Debug.LogWarning("Reached end of recursive call, hallway could not be created");
                //return false;
            }
        
            IEnumerator CreateChainOfHallwaysBetweenRoomsRoutine(GameObject startRoom, GameObject destRoom, CallbackDelegate callback)
            {
                if(roomsWithinRange[startRoom].Contains(destRoom))
                {
                    //If startRoom and destRoom can be connected by a hallway, try to connect them
                    bool isRunning = true;
                    StartCoroutine(CreateHallwayBetweenRoomsRoutine(startRoom, destRoom, Vector3.Distance(startRoom.transform.position, destRoom.transform.position), () => {isRunning = false;}));
                    yield return new WaitWhile(() => isRunning);
                    //Debug.LogWarning("Successfully connected " + startRoom + " to " + destRoom + ": " + returnValue);
                }
                else
                {
                    //For each room we can connect startRoom to, see if we can create a chain that leads to destRoom
                    GameObject nextClosestRoom = null;
                    bool connectionSuccessful = false;

                    while(!connectionSuccessful && roomsWithinRange[startRoom].Count > 0)
                    {
                        //Fring the closest room to destRoom
                        nextClosestRoom = FindClosestTransformToDestination(new List<Transform>(roomsWithinRange[startRoom].Select((GameObject original) => original.transform)), destRoom.transform).gameObject;

                        //Whether it succeeds or not, we want to remove the rooms from each other's sets of valid rooms so we know not to try to connect them again later
                        RemoveKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, ref startRoom, ref nextClosestRoom);

                        //Attempt to create a chain between this room and destRoom
                        bool isRunning = true;
                        StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(nextClosestRoom, destRoom, () => {isRunning = false;}));
                        yield return new WaitWhile(() => isRunning);
                        connectionSuccessful = returnValue;
                        //Debug.LogWarning("Successfully chained from " + nextClosestRoom + " to " + destRoom + ": " + connectionSuccessful);
                    } 

                    if(connectionSuccessful && nextClosestRoom != null)
                    {
                        bool isRunning = true;
                        StartCoroutine(CreateHallwayBetweenRoomsRoutine(startRoom, nextClosestRoom, Vector3.Distance(nextClosestRoom.transform.position, destRoom.transform.position), () => {isRunning = false;}));
                        yield return new WaitWhile(() => isRunning);
                        connectionSuccessful = returnValue;
                        //Debug.LogWarning("Successfully connected " + startRoom + " to " + nextClosestRoom + ": " + connectionSuccessful);
                    }

                    returnValue = connectionSuccessful;
                }

                callback();
            }

            void UndoGeneration()
            {
                foreach(GameObject room in encounterRooms)
                {
                    Destroy(room);
                }
                foreach(GameObject room in hallwayRooms)
                {
                    Destroy(room);
                }

                Destroy(spawnRoom);
                Destroy(xShop);
                Destroy(zShop);
                Destroy(centerShop);
                Destroy(bossRoom);
            }

            //Start by ensuring there's one guaranteed path from spawn to center shop, and then from center shop to edge shops and boss room
            bool coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(spawnRoom, centerShop, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogWarning("Procedural Generation Failed: Could not create a path from spawn room to center shop.");
                UndoGeneration();
                yield return null;
                successCallback(false);
                yield break;
            }

            coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(xShop, centerShop, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogWarning("Procedural Generation Failed: Could not create a path from center shop to secondary shop.");
                UndoGeneration();
                yield return null;
                successCallback(false);
                yield break;
            }

            coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(zShop, centerShop, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogWarning("Procedural Generation Failed: Could not create a path from center shop to secondary shop.");
                UndoGeneration();
                yield return null;
                successCallback(false);
                yield break;
            }
            
            coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(bossRoom, centerShop, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogWarning("Procedural Generation Failed: Could not create a path from center shop to boss room.");
                UndoGeneration();
                yield return null;
                successCallback(false);
                yield break;
            }

            //Iterate through pairs of nearby rooms and connect them with hallways
            List<GameObject> tempkeys = new List<GameObject>(roomsWithinRange.Keys);
            for(int i = 0; i < tempkeys.Count; ++i)
            {
                GameObject startRoom = tempkeys[i];

                //Because we're removing the keys and values from the dictionary rather than the list, we have to double check if it's still there
                if(!roomsWithinRange.ContainsKey(startRoom))
                    continue;

                List<GameObject> values = new List<GameObject>(roomsWithinRange[tempkeys[i]]);
                for(int j = 0; j < values.Count; ++j)
                {
                    GameObject destRoom = values[j];

                    //Because we're removing the keys and values from the dictionary rather than the list, we have to double check if it's still there
                    if(!roomsWithinRange[tempkeys[i]].Contains(destRoom))
                        continue;

                    //Try to connect the rooms and whether or not we succeed, we remove them from the dictionary
                    coroutineIsRunning = true;
                    StartCoroutine(CreateHallwayBetweenRoomsRoutine(startRoom, destRoom, Vector3.Distance(startRoom.transform.position, destRoom.transform.position), () => coroutineIsRunning = false));
                    yield return new WaitWhile(() => coroutineIsRunning);
                    // RemoveKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, ref startRoom, ref destRoom);
                    roomsWithinRange[startRoom].Remove(destRoom);

                    if(!returnValue)
                        Debug.LogWarning("Failed to create hallway between " + startRoom + " and " + destRoom);
                }
            }

            //Create and bake NavMesh
            GameObject navMeshObject = Instantiate(navMeshPrefab, Vector3.zero, Quaternion.identity);
            foreach(NavMeshSurface surface in navMeshObject.GetComponents<NavMeshSurface>())
            {
                surface.BuildNavMesh();
            }

            //Create organizational gameObjects
            GameObject encounterRoomOrganizer = new GameObject("Encounter Rooms");
            GameObject hallwayRoomOrganizer = new GameObject("Hallway Rooms");

            //Spawn hallway enemies and group hallways into a parent object
            foreach(GameObject hallway in hallwayRooms)
            {
                EnemyGen gen = hallway.GetComponentInChildren<EnemyGen>();
                if(gen != null)
                    gen.spawnEnemies();

                hallway.transform.parent = hallwayRoomOrganizer.transform;
            }

            //Hook up encounter rooms with the correct forcefield exits
            foreach(GameObject encounterRoom in encounterRooms)
            {
                Room room = encounterRoom.GetComponentInChildren<Room>();
                EncounterRoomClose roomClose = encounterRoom.GetComponentInChildren<EncounterRoomClose>();
                for(int i = 0; i < room.roomExits.Count; ++i)
                {
                    Transform exit = room.roomExits[i];
                    if(!exit.gameObject.activeSelf)
                    {
                        GameObject forceField = Instantiate(forceFieldPrefab, exit.position, exit.rotation, exit.parent);
                        roomClose.AddForceField(forceField);
                        forceField.SetActive(false);
                        Destroy(exit.gameObject);
                    }
                }
                encounterRoom.transform.parent = encounterRoomOrganizer.transform;
            }

            //Hook up boss room with the correct forcefield exit
            Room bossRoomScript = bossRoom.GetComponentInChildren<Room>();
            EncounterRoomClose bossRoomClose = bossRoom.GetComponentInChildren<EncounterRoomClose>();
            bossRoomClose.boi = beetleBoss;
            for(int i = 0; i < bossRoomScript.roomExits.Count; ++i)
            {
                Transform exit = bossRoomScript.roomExits[i];
                if(!exit.gameObject.activeSelf)
                {
                    GameObject forceField = Instantiate(forceFieldPrefab, exit.position, exit.rotation, exit.parent);
                    bossRoomClose.AddForceField(forceField);
                    forceField.SetActive(false);
                    Destroy(exit.gameObject);
                }
            }

            successCallback(true);
        }

        StartCoroutine(GenerateAllHallwaysRoutine());
        return;

    #endregion
          
#endregion
    
    }

    public IEnumerator GenerateUntilSuccessful()
    {
        bool success = false;
        
        while(!success)
        {
            bool waiting = true;
            Generate((bool generationSuccess) => {
                waiting = false;
                success = generationSuccess;
            });

            while(waiting)
            {
                yield return null;
            }
        }

        OnGenerationComplete.Invoke();
    }

    private HashSet<GameObject> FindRoomsWithinSphere(GameObject room, float radius)
    {
        //Using a hashset guarantees there will be no duplicate rooms
        HashSet<GameObject> rooms = new HashSet<GameObject>();

        foreach(Collider col in Physics.OverlapSphere(room.transform.position, radius * gridCellSizeInUnits, LayerMask.GetMask("RoomBounds")))
        {
            if(!col.transform.IsChildOf(room.transform))
            {
                Transform grandestParent = col.transform;
                while(grandestParent.parent != null)
                    grandestParent = grandestParent.parent;

                rooms.Add(grandestParent.gameObject);
            }
        }
        
        return rooms;
    }

    private HashSet<GameObject> FindNewRoomsWithinSphere(Dictionary<GameObject, HashSet<GameObject>> roomsWithinRange, GameObject room, float startRadius, float maxRadius)
    {
        //Guarantee that there's an encounter room that hasn't been added before
        HashSet<GameObject> validRooms = new HashSet<GameObject>();
        float expandingRadius = startRadius;
        do
        {
            validRooms = FindRoomsWithinSphere(room, expandingRadius);
            foreach(GameObject validRoom in new List<GameObject>(validRooms))
            {
                if(roomsWithinRange.ContainsKey(room) && roomsWithinRange[room].Contains(validRoom))
                    validRooms.Remove(validRoom);
            }
            ++expandingRadius;
        }
        while(validRooms.Count == 0 && expandingRadius < maxRadius);

        return validRooms;
    }

    private void AddKeyValuePairReflexively<T, C>(Dictionary<T, C> dictionary, T key, T value) where C : ICollection<T>, new()
    {
        if(key.Equals(value))
            return;

        if(!dictionary.ContainsKey(key))
            dictionary.Add(key, new C());

        if(!dictionary.ContainsKey(value))
            dictionary.Add(value, new C());
        
        dictionary[key].Add(value);
        dictionary[value].Add(key);
    }

    private void RemoveKeyValuePairReflexively<T, C>(Dictionary<T, C> dictionary, ref T key, ref T value) where C : ICollection<T>
    {
        if(dictionary.ContainsKey(key))
            dictionary[key].Remove(value);

        if(dictionary.ContainsKey(value))
            dictionary[value].Remove(key);
    }

    private Transform FindClosestTransformToDestination<C>(C potentialStartPoints, Transform destination, Vector3? offsetAxis = null, float offsetDistance = 0) where C : ICollection<Transform>
    {
        if(potentialStartPoints == null || potentialStartPoints.Count == 0 || destination == null) return null;

        Vector3 offset;
        if(offsetAxis.HasValue)
            offset = offsetAxis.Value.normalized;
        else
            offset = -Vector3.up;

        Transform closest = potentialStartPoints.ElementAt(0);
        float distance = Vector3.Distance(closest.position + closest.rotation*offset*offsetDistance, 
                                          destination.position + destination.rotation*offset*offsetDistance);
        foreach(Transform t in potentialStartPoints)
        {
            float newDistance = Vector3.Distance(t.position + t.rotation*offset*offsetDistance, 
                                                 destination.position + destination.rotation*offset*offsetDistance);
            if(newDistance < distance)
            {
                closest = t;
                distance = newDistance;
            }
        }
        return closest;
    }

    private KeyValuePair<Transform, Transform> FindClosestTransformPair<C,D>(C potentialStartPoints, D potentialDestinations, Vector3? offsetAxis = null, float offsetDistance = 0) where C : ICollection<Transform> where D : ICollection<Transform>
    {
        if(potentialStartPoints == null || potentialStartPoints.Count == 0 || potentialDestinations == null || potentialDestinations.Count == 0) 
            return new KeyValuePair<Transform, Transform>(null, null);

        Transform closestDestination = potentialDestinations.ElementAt(0);
        Transform closestStart = potentialStartPoints.ElementAt(0);

        Vector3 offset;
        if(offsetAxis.HasValue)
            offset = offsetAxis.Value.normalized;
        else
            offset = -Vector3.up;

        float distance = Vector3.Distance(closestStart.position + closestStart.rotation*offset*offsetDistance, 
                                          closestDestination.position + closestDestination.rotation*offset*offsetDistance);
        foreach(Transform tDest in potentialDestinations)
        {
            Transform tStart = FindClosestTransformToDestination(potentialStartPoints, tDest, offsetAxis, offsetDistance);
            float newDistance = Vector3.Distance(tStart.position + tStart.rotation*offset*offsetDistance, 
                                                 tDest.position + tDest.rotation*offset*offsetDistance);
            if(newDistance < distance)
            {
                closestDestination = tDest;
                closestStart = tStart;
                distance = newDistance;
            }
        }
        return new KeyValuePair<Transform, Transform>(closestStart, closestDestination);
    }
}