using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentRunNumber {get; private set;}

    private InputManager inputManager;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private GearManagerObject gearManager;

    [HideInInspector] public bool playerDeath = false;
    [HideInInspector] public int bossesKilled = 0;

    // Set to true (permanently) once you have killed the Time Lich at least once
    // (Makes a new special item pop up in Stellan's shop)
    [HideInInspector] public bool hasKilledTimeLich = false;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void Start()
    {
        currentRunNumber = 1;
    }

    private void Update()
    {
        if(inputManager == null)
        {
            inputManager = GameObject.FindWithTag("Player").GetComponent<InputManager>();
        }
        if(playerDeath)
        {
            InGameUIManager.instance.deathScreen.OpenPlayerDeathUI();
            playerDeath = false;
        }
    }

    // Add any other reset stuff here too (called when player goes from death screen -> main hub)
    public void EndRun()
    {
        // Unequip all items, clear temp currency, clear potions
        PlayerInventory.instance.ClearRunInventory();

        // Set all NPC talked to variables to false and increment story beats
        StoryManager.instance.OnRunEndUpdateStory();
        
        // Increment run # -> once in main hub, run # always = # of your NEXT run (not previous)
        currentRunNumber++;
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
        if (File.Exists(Application.persistentDataPath + "/gamesave.save")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // TODO: Set values according to the save file

            Debug.Log("Game Loaded");
        }
        else{
            Debug.Log("No game saved!");
        }
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Main Hub"){
            InGameUIManager.instance.ToggleRunUI(false);
        }
    }

    public GearManagerObject GearManager()
    {
        return gearManager;
    }
}
