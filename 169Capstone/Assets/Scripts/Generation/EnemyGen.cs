using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGen : MonoBehaviour
{
    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int maxTarget;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject robertPrefab;
    [SerializeField] private Room roomScript;
    [SerializeField] private int offsetRadius = 3;
    private int slimeCount;
    private int robertCount;

    void Awake()
    {
        if(spawnOnStart)
            spawnEnemies();
    }

    public void spawnEnemies()
    {
        int upperRange = Mathf.CeilToInt(maxTarget / 2);
        int lowerRange = Mathf.FloorToInt(maxTarget / 2);
        System.Random r = new System.Random();
        int rand = r.Next(0, 2);

        if (rand == 0)
        {
            slimeCount = upperRange;
        }
        else
        {
            slimeCount = lowerRange;
        }

        robertCount = maxTarget - slimeCount;

        for (int i = 0; i < slimeCount; i++)
        {
            Transform spawnPoint = spawnPoints[r.Next(0, spawnPoints.Count)];
            GameObject enemy = Instantiate(slimePrefab, spawnPoint.position + new Vector3(r.Next(-1 * offsetRadius, offsetRadius), 0, r.Next(-1 * offsetRadius, offsetRadius)), slimePrefab.transform.rotation);
            roomScript.AddEnemy(enemy.GetComponent<EntityHealth>());
        }
        for (int i = 0; i < robertCount; i++)
        {
            Transform spawnPoint = spawnPoints[r.Next(0, spawnPoints.Count)];
            GameObject enemy = Instantiate(robertPrefab, spawnPoint.position + new Vector3(r.Next(-1 * offsetRadius, offsetRadius), 0, r.Next(-1 * offsetRadius, offsetRadius)), robertPrefab.transform.rotation);
            roomScript.AddEnemy(enemy.GetComponent<EntityHealth>());
        }
    }
}
