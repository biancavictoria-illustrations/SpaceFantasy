using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private List<Transform> roomExits;
    private HashSet<EntityHealth> enemies;

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
        if(enemies == null)
        {
            enemies = new HashSet<EntityHealth>();
        }

        enemies.Add(enemy);
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
}
