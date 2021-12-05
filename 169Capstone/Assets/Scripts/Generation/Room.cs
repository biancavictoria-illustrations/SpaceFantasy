using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private List<EntityHealth> startingEnemies;
    private HashSet<EntityHealth> enemies;

    void Awake()
    {
        enemies = new HashSet<EntityHealth>(startingEnemies);
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
    }

    public bool hasEnemies()
    {
        return enemies.Count > 0;
    }
}
