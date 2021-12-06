using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Dictionary<string, GameObject[]> gear = new Dictionary<string, GameObject[]>();
    private Dictionary<string, string[]> titles = new Dictionary<string, string[]>();
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GearManagerObject gearManager;
    [SerializeField] private TitleManagerObject titleManager;
    [SerializeField] private Transform playerTransform;

    [HideInInspector] public bool playerDeath = false;
    [HideInInspector] public int bossesKilled = 0;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    private void Start()
    {
        gear.Add("Weapon", gearManager.weapons);
        gear.Add("Accessory", gearManager.accessories);
        gear.Add("Head", gearManager.head);
        gear.Add("Leg", gearManager.legs);

        titles.Add("Weapon", titleManager.weapons);
        titles.Add("Accessory", titleManager.accessories);
        titles.Add("Head", titleManager.head);
        titles.Add("Leg", titleManager.legs);

        Instantiate(playerPrefab, playerTransform.position, playerTransform.rotation);
    }

    private void Update()
    {
        if(playerDeath)
        {
            InGameUIManager.instance.deathScreen.OpenPlayerDeathUI();
        }
    }

    public void StartNewRun()
    {
        
    }

    public void ChangeScene()
    {
        SaveGame();
    }

    private Save CreateSaveGameObject()
    {
        Save saveFile = new Save();
        
        // TODO: Store the data (set all the variables in the Save class according to the current game state)

        return saveFile;
    }

    public void SaveGame()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
        
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // TODO: Set values according to the save file

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }

    public GameObject GetGearObject(string title, string gearType)
    {
        int index = System.Array.IndexOf(titles[gearType], title);
        return Instantiate(gear[gearType][index]);
    }
}
