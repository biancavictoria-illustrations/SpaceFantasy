using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private List<EntityHealth> startingEnemies;
    [SerializeField] private List<Transform> roomExits;
    public HashSet<EntityHealth> enemies{get; private set;}

    void Awake()
    {
        for(int i = 0; i < startingEnemies.Count; ++i)
        {
            if(startingEnemies[i] == null)
            {
                startingEnemies.RemoveAt(i);
                --i;
            }
        }

        foreach(Collider col in GetComponentsInChildren<Collider>())
        {
            Collider[] overlaps = Physics.OverlapBox(col.bounds.center, col.bounds.extents, transform.rotation, LayerMask.GetMask("Enemy"));
            foreach(Collider enemyCol in overlaps)
            {
                EntityHealth health = enemyCol.GetComponent<EntityHealth>();
                if(health != null)
                {
                    startingEnemies.Add(health);
                }
            }
        }

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
        Debug.Log("Enemy Update");
    }

    public bool hasEnemies()
    {
        return enemies.Count > 0;
    }
    
    
}
