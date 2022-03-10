﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGen : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private int maxTarget;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject robertPrefab;
    private int slimeCount;
    private int robertCount;

    // Start is called before the first frame update
    void Start()
    {
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
            GameObject enemy = Instantiate(slimePrefab, spawnPoint);
            enemy.transform.rotation = slimePrefab.transform.rotation;
        }
        for (int i = 0; i < robertCount; i++)
        {
            Transform spawnPoint = spawnPoints[r.Next(0, spawnPoints.Count)];
            GameObject enemy = Instantiate(robertPrefab, spawnPoint);
            enemy.transform.rotation = robertPrefab.transform.rotation;
        }
    }
}
