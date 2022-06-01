using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// MIGHT BE DEPRECATED??????

public class ObjectManager : MonoBehaviour
{
    // gear and titles should be in the same order
    [SerializeField] private GameObject[] gear;
    private string[] titles = new string[] { "Berserker's Zweihander", "Bow And Arrows" };
    public bool playerDeath = false;
    //[SerializeField] private Transform mainHub;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string mainHub;
    public static int bossesKilled = 0;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(playerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerDeath)
        {
            //Instantiate(playerPrefab, mainHub);
            // SceneManager.LoadScene(mainHub);

            InGameUIManager.instance.deathScreen.OpenPlayerDeathUI();
        }
    }

    public GameObject GetGearObject(string title)
    {
        int index = System.Array.IndexOf(titles, title);
        return Instantiate(gear[index]);
    }
}
