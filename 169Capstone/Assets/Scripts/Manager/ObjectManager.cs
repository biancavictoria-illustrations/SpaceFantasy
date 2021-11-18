using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // gear and titles should be in the same order
    [SerializeField] private GameObject[] gear;
    private string[] titles = new string[] { "Berserker's Zweihander", "Bow And Arrows" };
    public bool playerDeath = false;
    [SerializeField] private Transform mainHub;
    [SerializeField] private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerDeath)
        {
            Instantiate(playerPrefab, mainHub);
        }
    }

    public GameObject GetGearObject(string title)
    {
        int index = System.Array.IndexOf(titles, title);
        return Instantiate(gear[index]);
    }
}
