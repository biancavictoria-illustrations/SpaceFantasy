using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorGenerator : MonoBehaviour
{
    private const int maxNumberOfPlacementAttempts = 100;
    
    private delegate void CallbackDelegate();

    [Tooltip("Generate a floor during the next Update (FOR TESTING ONLY)")]
    public bool generate = false;

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
    [Tooltip("The material to be applied to the force fields that trap players inside an encounter room.")]
    public Material forceFieldMat;

    public GameObject spawnRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject shopRoomPrefab;

    [Tooltip("A list of all room prefabs which will trap the player inside until they defeat all enemies.")]
    public List<GameObject> encounterRoomPrefabs;
    [Tooltip("A list of all room prefabs which will connect encounter rooms together.")]
    public List<GameObject> hallwayRoomPrefabs;


    void Start()
    {
        if(generateOnStart)
            Generate();
    }

    void Update()
    {
        if(generate)
        {
            Generate();
            generate = false;
        }
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

                //Instantiate the room at the chosen coordinates and grab its Room script and its bounds colliders
                GameObject currentRoom = Instantiate(prefab, position, rotation);
                Room roomScript = currentRoom.GetComponentInChildren<Room>();
                HashSet<BoxCollider> roomBoundsList = new HashSet<BoxCollider>(roomScript.GetComponents<BoxCollider>());

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

    #region Hallway Generation Methods

        //The bulk of hallway generation
        bool CreateHallwayBetweenRooms(GameObject startRoom, GameObject destRoom)
        {
            //Get the exit on destRoom that is closest to startRoom
            List<Transform> destRoomExits = new List<Transform>(destRoom.GetComponentInChildren<Room>().roomExits);
            Transform destExit = destRoomExits[0];
            float shortestDistance = Vector3.Distance(destExit.position, startRoom.transform.position);
            foreach(Transform exit in destRoomExits)
            {
                if(!exit.gameObject.activeSelf) continue;

                float distance = Vector3.Distance(exit.position, startRoom.transform.position);
                if(distance < shortestDistance)
                {
                    destExit = exit;
                    shortestDistance = distance;
                }
            }

            //If all of the exits are already taken, give up
            if(!destExit.gameObject.activeSelf) return false;

            //Get the list of exits on startRoom
            List<Transform> roomExits = new List<Transform>(startRoom.GetComponentInChildren<Room>().roomExits);
            bool successfulHallway = false;

            //Try to connect to destExit with each of startRoom's exits, in order of which exit is closest
            do
            {
                //Find the closest exit to destExit
                Transform closestExit = roomExits[0];
                shortestDistance = Vector3.Distance(closestExit.position, destExit.position);
                foreach(Transform exit in roomExits)
                {
                    if(!exit.gameObject.activeSelf) continue;

                    float distance = Vector3.Distance(exit.position, destExit.position);
                    if(distance < shortestDistance)
                    {
                        closestExit = exit;
                        shortestDistance = distance;
                    }
                }

                //If all of the exits are already taken, give up
                if(!closestExit.gameObject.activeSelf) return false;

                roomExits.Remove(closestExit);

                //Keep picking hallway pieces until we find one that connects to destExit
                List<GameObject> hallwayPrefabs = new List<GameObject>(hallwayRoomPrefabs);
                do
                {
                    //Pick and instantiate a random hallway piece
                    int index = Random.Range(0, hallwayPrefabs.Count);
                    GameObject prefab = hallwayPrefabs[index];
                    hallwayPrefabs.RemoveAt(index);
                    GameObject hallwayPiece = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                    HashSet<BoxCollider> roomBoundsList = new HashSet<BoxCollider>();

                    //Try each orientation of the hallway until we find one that gets us closer to destExit
                    List<Transform> hallwayExits = hallwayPiece.GetComponentInChildren<Room>().roomExits;
                    for(int i = 0; i < hallwayExits.Count; ++i)
                    {
                        Transform hallwayExit = hallwayExits[i];

                        //Rotate the hallway to match closestExit
                        Quaternion orientation = Quaternion.FromToRotation(hallwayExit.up, -closestExit.up);
                        hallwayPiece.transform.rotation = orientation * hallwayPiece.transform.rotation;

                        //Move the hallway so exit matches closestExit
                        Vector3 exitToCenter = hallwayPiece.transform.position - hallwayExit.position;
                        hallwayPiece.transform.position = closestExit.position + exitToCenter;

                        //Check if the hallway piece overlaps another room
                        Room roomScript = hallwayPiece.GetComponentInChildren<Room>();
                        roomBoundsList = new HashSet<BoxCollider>(roomScript.GetComponents<BoxCollider>());

                        //If any of the colliders intersect another room, try again
                        bool intersects = false;
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
                                    Debug.Log(hallwayPiece + " intersects " + room.transform.parent.gameObject);
                                    intersects = true;
                                    break;
                                }
                            }

                            if(intersects)
                                break;
                        }

                        //If the room overlaps another room, try a different orientation. 
                        //If we run out of possible orientations the algorithm will move on to a new hallway prefab
                        if(intersects)
                        {
                            continue;
                        }

                        //Find the closest exit from the hallway to destExit (can't be exit since that's connected to)
                        Transform hallwayDestExit = hallwayExits[0];
                        if(hallwayDestExit == hallwayExit)
                            hallwayDestExit = hallwayExits[1];

                        float hallwayDistance = Vector3.Distance(hallwayDestExit.position, destExit.position);
                        foreach(Transform otherExit in hallwayPiece.GetComponentInChildren<Room>().roomExits)
                        {
                            if(otherExit == hallwayDestExit) continue;

                            float distance = Vector3.Distance(otherExit.position, destExit.position);
                            if(distance < hallwayDistance)
                            {
                                hallwayDestExit = otherExit;
                                hallwayDistance = distance;
                            }
                        }

                        //If the distance is negligible we finally created a working hallway
                        if(hallwayDistance < 0.1f)
                        {
                            //Remove the walls at the exits
                            hallwayExit.gameObject.SetActive(false);
                            hallwayDestExit.gameObject.SetActive(false);
                            destExit.gameObject.SetActive(false);
                            return true;
                        }
                        else if(hallwayDistance < shortestDistance) //Only continue the hallway if hallwayExit is closer to destExit than closestExit
                        {
                            successfulHallway = CreateHallwayBetweenRooms(hallwayPiece, destRoom);
                            if(successfulHallway)
                            {
                                hallwayExit.gameObject.SetActive(false);
                                hallwayDestExit.gameObject.SetActive(false);
                                break;
                            }
                        }
                    }
                    
                    if(!successfulHallway)
                    {
                        foreach(BoxCollider roomBounds in roomBoundsList)
                        {
                            roomBounds.enabled = false;
                        }

                        Debug.Log("Could not place hallway " + hallwayPiece + " connected to " + startRoom);
                        Destroy(hallwayPiece);
                    }
                }
                while(!successfulHallway && hallwayPrefabs.Count > 0);

                //If that exit worked, remove it
                if(successfulHallway)
                    closestExit.gameObject.SetActive(false);
            }
            while(!successfulHallway && roomExits.Count > 0);

            return successfulHallway;
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
                GameObject nextClosestRoom = null;
                bool connectionSuccessful = false;

                do
                {
                    if(roomsWithinRange[startRoom].Count == 0)
                        break;

                    //Pick an arbitrary room from the set of valid rooms
                    nextClosestRoom = roomsWithinRange[startRoom].ElementAt(0);
                    float shortestDistance = Vector3.Distance(nextClosestRoom.transform.position, destRoom.transform.position);

                    //Find the closest valid room to destRoom
                    foreach(GameObject room in roomsWithinRange[startRoom])
                    {
                        float distance = Vector3.Distance(room.transform.position, destRoom.transform.position);
                        if(distance < shortestDistance)
                        {
                            nextClosestRoom = room;
                            shortestDistance = distance;
                        }
                    }

                    //Whether it succeeds or not, we want to remove the rooms from each other's sets of valid rooms so we know not to try to connect them again later
                    RemoveKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, ref startRoom, ref nextClosestRoom);

                    //Attempt to create a chain between this room and destRoom
                    connectionSuccessful = CreateChainOfHallwaysBetweenRooms(nextClosestRoom, destRoom);
                } 
                while(!connectionSuccessful && roomsWithinRange[startRoom].Count > 0);

                if(connectionSuccessful && nextClosestRoom != null)
                    connectionSuccessful = CreateHallwayBetweenRooms(startRoom, nextClosestRoom);
                
                return connectionSuccessful;
            }
        }

    #endregion

    #region TESTING PLEASE DELETE

        IEnumerator PleaseDeleteThisCoroutine()
        {
            bool returnValue = false;

            IEnumerator CreateHallwayBetweenRoomsRoutine(GameObject startRoom, GameObject destRoom, CallbackDelegate callback)
            {
                //Get the exit on destRoom that is closest to startRoom
                List<Transform> destRoomExits = new List<Transform>(destRoom.GetComponentInChildren<Room>().roomExits);
                Transform destExit = destRoomExits[0];
                float shortestDistance = Vector3.Distance(destExit.position, startRoom.transform.position);
                foreach(Transform exit in destRoomExits)
                {
                    if(!exit.gameObject.activeSelf) continue;

                    float distance = Vector3.Distance(exit.position, startRoom.transform.position);
                    if(distance < shortestDistance)
                    {
                        destExit = exit;
                        shortestDistance = distance;
                    }
                }

                //If all of the exits are already taken, give up
                if(!destExit.gameObject.activeSelf) 
                {
                    returnValue = false;
                    yield break;
                }

                //Get the list of exits on startRoom
                List<Transform> roomExits = new List<Transform>(startRoom.GetComponentInChildren<Room>().roomExits);
                bool successfulHallway = false;

                //Try to connect to destExit with each of startRoom's exits, in order of which exit is closest
                do
                {
                    //Find the closest exit to destExit
                    Transform closestExit = roomExits[0];
                    shortestDistance = Vector3.Distance(closestExit.position, destExit.position);
                    foreach(Transform exit in roomExits)
                    {
                        if(!exit.gameObject.activeSelf) continue;

                        float distance = Vector3.Distance(exit.position, destExit.position);
                        if(distance < shortestDistance)
                        {
                            closestExit = exit;
                            shortestDistance = distance;
                        }
                    }

                    //If all of the exits are already taken, give up
                    if(!closestExit.gameObject.activeSelf) 
                    {
                        returnValue = false;
                        yield return null;
                    }

                    roomExits.Remove(closestExit);

                    //Keep picking hallway pieces until we find one that connects to destExit
                    List<GameObject> hallwayPrefabs = new List<GameObject>(hallwayRoomPrefabs);
                    do
                    {
                        //Pick and instantiate a random hallway piece
                        int index = Random.Range(0, hallwayPrefabs.Count);
                        GameObject prefab = hallwayPrefabs[index];
                        hallwayPrefabs.RemoveAt(index);
                        GameObject hallwayPiece = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                        HashSet<BoxCollider> roomBoundsList = new HashSet<BoxCollider>();

                        //Try each orientation of the hallway until we find one that gets us closer to destExit
                        List<Transform> hallwayExits = hallwayPiece.GetComponentInChildren<Room>().roomExits;
                        for(int i = 0; i < hallwayExits.Count; ++i)
                        {
                            Transform hallwayExit = hallwayExits[i];

                            //ADD TO ALGORITHM
                            if(!hallwayExit.gameObject.activeSelf)
                                continue;

                            //Rotate the hallway to match closestExit
                            Quaternion orientation = Quaternion.FromToRotation(hallwayExit.up, -closestExit.up);
                            hallwayPiece.transform.rotation = orientation * hallwayPiece.transform.rotation;

                            //Move the hallway so exit matches closestExit
                            Vector3 exitToCenter = hallwayPiece.transform.position - hallwayExit.position;
                            hallwayPiece.transform.position = closestExit.position + exitToCenter;

                            //Check if the hallway piece overlaps another room
                            Room roomScript = hallwayPiece.GetComponentInChildren<Room>();
                            roomBoundsList = new HashSet<BoxCollider>(roomScript.GetComponents<BoxCollider>());

                            //If any of the colliders intersect another room, try again
                            bool intersects = false;
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
                                        Debug.Log(hallwayPiece + " intersects " + room.transform.parent.gameObject);
                                        intersects = true;
                                        break;
                                    }
                                }

                                if(intersects)
                                    break;
                            }
                            
                            Debug.Log("Check for intersect");
                            yield return null;

                            //If the room overlaps another room, try a different orientation. 
                            //If we run out of possible orientations the algorithm will move on to a new hallway prefab
                            if(intersects)
                            {
                                continue;
                            }

                            //Find the closest exit from the hallway to destRoom (can't be hallwayExit since that's connected to startRoom)
                            Transform hallwayDestExit = hallwayExits[0];
                            if(hallwayDestExit == hallwayExit || !hallwayDestExit.gameObject.activeSelf)
                                hallwayDestExit = hallwayExits[1];

                            float hallwayDistance = Vector3.Distance(hallwayDestExit.position, destRoom.transform.position);
                            foreach(Transform otherExit in hallwayPiece.GetComponentInChildren<Room>().roomExits)
                            {
                                //ADD TO ALGORITHM
                                if(otherExit == hallwayDestExit || !otherExit.gameObject.activeSelf) continue;

                                float distance = Vector3.Distance(otherExit.position, destRoom.transform.position);
                                if(distance < hallwayDistance)
                                {
                                    hallwayDestExit = otherExit;
                                    hallwayDistance = distance;
                                }
                            }

                            //ADD TO ALGORITHM
                            if(!hallwayDestExit.gameObject.activeSelf) 
                            {
                                continue;
                            }

                            Debug.Log("Previous hallwayDistance: " + hallwayDistance);
                            Debug.Log("Previous destExit: " + destExit);

                            //ADD ALL THIS CODE TO ALGORITHM
                            //Re-evaluate the exit that is closest to the hallwayDestExit
                            float newShortestDistance = Vector3.Distance(destExit.position, hallwayDestExit.position);
                            foreach(Transform exit in destRoomExits)
                            {
                                if(!exit.gameObject.activeSelf) continue;

                                float distance = Vector3.Distance(exit.position, hallwayDestExit.position);
                                if(distance < newShortestDistance)
                                {
                                    destExit = exit;
                                    newShortestDistance = distance;
                                }
                            }

                            hallwayDistance = Vector3.Distance(hallwayDestExit.position, destExit.position);
                            //ADD ALL THIS CODE TO ALGORITHM

                            Debug.Log("New hallwayDistance: " + hallwayDistance);
                            Debug.Log("New destExit: " + destExit);

                            //If the distance is negligible we finally created a working hallway
                            if(hallwayDistance < 0.1f)
                            {
                                //Remove the walls at the exits
                                closestExit.gameObject.SetActive(false); //ADD THESE TO MAIN ALGORITHM
                                hallwayExit.gameObject.SetActive(false);
                                hallwayDestExit.gameObject.SetActive(false);
                                destExit.gameObject.SetActive(false); //ADD THESE TO MAIN ALGORITHM
                                Debug.Log("Successful Hallway");
                                yield return null;
                                returnValue = true;
                                callback();
                                yield break;
                            }
                            else if(hallwayDistance < shortestDistance) //Only continue the hallway if hallwayExit is closer to destExit than closestExit
                            {
                                bool isRunning = true;
                                hallwayExit.gameObject.SetActive(false); //ADD THESE TO MAIN ALGORITHM
                                closestExit.gameObject.SetActive(false); //ADD THESE TO MAIN ALGORITHM
                                Debug.Log("Case hallwayDistance < shortestDistance");
                                yield return null;
                                StartCoroutine(CreateHallwayBetweenRoomsRoutine(hallwayPiece, destRoom, () => {isRunning = false;}));
                                yield return new WaitWhile(() => isRunning);
                                Debug.Log("Return Value: " + returnValue);
                                successfulHallway = returnValue;
                                yield return null;
                                if(!successfulHallway) //THIS WAS CHANGED TOO
                                {
                                    hallwayExit.gameObject.SetActive(true);
                                    closestExit.gameObject.SetActive(true);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        
                        if(!successfulHallway)
                        {
                            foreach(BoxCollider roomBounds in roomBoundsList)
                            {
                                roomBounds.enabled = false;
                            }

                            Debug.Log("Could not place hallway " + hallwayPiece + " connected to " + startRoom);
                            yield return null;
                            Destroy(hallwayPiece);
                        }
                    }
                    while(!successfulHallway && hallwayPrefabs.Count > 0);

                    //THERE WAS CODE HERE THAT NEEDS TO BE REMOVED
                }
                while(!successfulHallway && roomExits.Count > 0);

                Debug.Log("Reached end of routine, successful hallway: " + successfulHallway);
                yield return null;

                returnValue = successfulHallway;
                callback();
            }
        
            IEnumerator CreateChainOfHallwaysBetweenRoomsRoutine(GameObject startRoom, GameObject destRoom, CallbackDelegate callback)
            {
                if(roomsWithinRange[startRoom].Contains(destRoom))
                {
                    //If startRoom and destRoom can be connected by a hallway, try to connect them
                    bool isRunning = true;
                    StartCoroutine(CreateHallwayBetweenRoomsRoutine(startRoom, destRoom, () => {isRunning = false;}));
                    yield return new WaitWhile(() => isRunning);
                }
                else
                {
                    //For each room we can connect startRoom to, see if we can create a chain that leads to destRoom
                    GameObject nextClosestRoom = null;
                    bool connectionSuccessful = false;

                    do
                    {
                        if(roomsWithinRange[startRoom].Count == 0)
                            break;

                        //Pick an arbitrary room from the set of valid rooms
                        nextClosestRoom = roomsWithinRange[startRoom].ElementAt(0);
                        float shortestDistance = Vector3.Distance(nextClosestRoom.transform.position, destRoom.transform.position);

                        //Find the closest valid room to destRoom
                        foreach(GameObject room in roomsWithinRange[startRoom])
                        {
                            float distance = Vector3.Distance(room.transform.position, destRoom.transform.position);
                            if(distance < shortestDistance)
                            {
                                nextClosestRoom = room;
                                shortestDistance = distance;
                            }
                        }

                        //Whether it succeeds or not, we want to remove the rooms from each other's sets of valid rooms so we know not to try to connect them again later
                        RemoveKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, ref startRoom, ref nextClosestRoom);

                        //Attempt to create a chain between this room and destRoom
                        bool isRunning = true;
                        StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(nextClosestRoom, destRoom, () => {isRunning = false;}));
                        yield return new WaitWhile(() => isRunning);
                        connectionSuccessful = returnValue;
                    } 
                    while(!connectionSuccessful && roomsWithinRange[startRoom].Count > 0);

                    if(connectionSuccessful && nextClosestRoom != null)
                    {
                        bool isRunning = true;
                        StartCoroutine(CreateHallwayBetweenRoomsRoutine(startRoom, nextClosestRoom, () => {isRunning = false;}));
                        yield return new WaitWhile(() => isRunning);
                        connectionSuccessful = returnValue;
                    }

                    returnValue = connectionSuccessful;
                }

                callback();
            }
        
            //Start by ensuring there's one guaranteed path from spawn to center shop, and then from center shop to edge shops and boss room
            bool coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(spawnRoom, centerShop, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogError("Procedural Generation Failed: Could not create a path from spawn room to center shop.");
            }

            coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(centerShop, xShop, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogError("Procedural Generation Failed: Could not create a path from center shop to secondary shop.");
            }

            coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(centerShop, zShop, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogError("Procedural Generation Failed: Could not create a path from center shop to secondary shop.");
            }
            
            coroutineIsRunning = true;
            StartCoroutine(CreateChainOfHallwaysBetweenRoomsRoutine(centerShop, bossRoom, () => coroutineIsRunning = false));
            yield return new WaitWhile(() => coroutineIsRunning);
            if(!returnValue)
            {
                Debug.LogError("Procedural Generation Failed: Could not create a path from center shop to boss room.");
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
                    StartCoroutine(CreateHallwayBetweenRoomsRoutine(startRoom, destRoom, () => coroutineIsRunning = false));
                    yield return new WaitWhile(() => coroutineIsRunning);
                    RemoveKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, ref startRoom, ref destRoom);

                    if(!returnValue)
                        Debug.LogWarning("Failed to create hallway between " + startRoom + " and " + destRoom);
                }
            }
        }

        StartCoroutine(PleaseDeleteThisCoroutine());
        return;

    #endregion
            
    #region Necessary Hallway Generation

        //Start by ensuring there's one guaranteed path from spawn to center shop, and then from center shop to edge shops and boss room
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

    #region Additional Hallway Generation

        //Iterate through pairs of nearby rooms and connect them with hallways
        List<GameObject> keys = new List<GameObject>(roomsWithinRange.Keys);
        for(int i = 0; i < keys.Count; ++i)
        {
            GameObject startRoom = keys[i];

            //Because we're removing the keys and values from the dictionary rather than the list, we have to double check if it's still there
            if(!roomsWithinRange.ContainsKey(startRoom))
                continue;

            List<GameObject> values = new List<GameObject>(roomsWithinRange[keys[i]]);
            for(int j = 0; j < values.Count; ++j)
            {
                GameObject destRoom = values[j];

                //Because we're removing the keys and values from the dictionary rather than the list, we have to double check if it's still there
                if(!roomsWithinRange[keys[i]].Contains(destRoom))
                    continue;

                //Try to connect the rooms and whether or not we succeed, we remove them from the dictionary
                bool connect = CreateHallwayBetweenRooms(startRoom, destRoom);
                RemoveKeyValuePairReflexively<GameObject, HashSet<GameObject>>(roomsWithinRange, ref startRoom, ref destRoom);

                if(!connect)
                    Debug.LogWarning("Failed to create hallway between " + startRoom + " and " + destRoom);
            }
        }

    #endregion

#endregion
    
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
}