using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviour
{
    // Set in the inspector
    [SerializeField] private List<GameObject> propPrefabs = new List<GameObject>();
    
    [Tooltip("Provide a percent chance of one of the props spawning (i.e. 25 for 25%, not 0.25)")]
    [SerializeField] private float chanceOfSpawn;

    void Start()
    {
        // Determine if we're going to spawn an object here
        if( Random.Range(1,100) <= chanceOfSpawn ){
            // Spawn a random prop from the options provided
            int index = Random.Range(0,propPrefabs.Count);

            // Instantiate the prop as a child of this game object
            Instantiate(propPrefabs[index], transform);
        }
    }
}
