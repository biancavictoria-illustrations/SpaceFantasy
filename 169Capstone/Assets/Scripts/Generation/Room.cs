using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Transform> roomExits;
    [Tooltip("X is room length along the X axis in grid units, Y is the same for the Z axis")]
    public Vector2Int roomSize;
    [Tooltip("Total number of grid units in the room")]
    public int numGridSpaces;
    public HashSet<EntityHealth> enemies{get; private set;}

    private bool roomHasBeenVisited = false;

    void Awake()
    {
        enemies = new HashSet<EntityHealth>();
    }

    void Start()
    {
        if(enemies == null)
            return;

        foreach(EntityHealth enemy in enemies)
        {
            if(!enemy.CompareTag("Player"))
            {
                enemy.OnDeath.AddListener(updateEnemies);
            }
        }
    }

    private void updateEnemies(EntityHealth health)
    {
        enemies.Remove(health);
        Debug.Log("Enemy Update");
    }

    public void AddEnemy(EntityHealth enemy)
    {
        Debug.Log("Enemy Added");

        if(enemies == null)
        {
            enemies = new HashSet<EntityHealth>();
        }

        enemies.Add(enemy);
        enemy.OnDeath.AddListener(updateEnemies);
    }

    public HashSet<EntityHealth> GetEnemyList()
    {
        if(enemies == null)
            return new HashSet<EntityHealth>();

        return enemies;
    }

    public bool hasEnemies()
    {
        if(enemies == null)
            return false;
            
        return enemies.Count > 0;
    }

    public void enableAllMinimapSprites()
    {
        // If we already activated the minimap sprites, return
        if(roomHasBeenVisited){
            return;
        }

        // Get all the minimap sprites in this room
        MinimapSprite[] minimapSprites = transform.parent.GetComponentsInChildren<MinimapSprite>();

        // If there aren't any, return
        if(minimapSprites.Length == 0){
            return;
        }

        // Otherwise, iterate through and activate all of them
        foreach(MinimapSprite ms in minimapSprites){
            ms.MinimapSpriteDiscovered();
        }

        roomHasBeenVisited = true;
    }
}
