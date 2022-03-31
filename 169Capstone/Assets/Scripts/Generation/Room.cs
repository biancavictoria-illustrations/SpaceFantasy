using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Transform> roomExits;
    public HashSet<EntityHealth> enemies{get; private set;}

    void Awake()
    {
        enemies = new HashSet<EntityHealth>();
    }

    void Start()
    {
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

    public bool hasEnemies()
    {
        return enemies.Count > 0;
    }
    
    
}
