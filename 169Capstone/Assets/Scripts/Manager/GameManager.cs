using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public const string MAIN_HUB_STRING_NAME = "Main Hub";
    public const string GAME_LEVEL_STRING_NAME = "TestFloor";   // TODO: Update this!!!

    public string currentSceneName {get; private set;}

    public const float DEFAULT_AUTO_DIALOGUE_WAIT_TIME = 1.1f;

    public int currentRunNumber {get; private set;}

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private GearManagerObject gearManager;

    [HideInInspector] public bool playerDeath = false;
    [HideInInspector] public int bossesKilled = 0;

    public GameTimer gameTimer;

    // Set to true (permanently) once you have killed the Time Lich at least once
    // (Makes a new special item pop up in Stellan's shop)
    [HideInInspector] public bool hasKilledTimeLich = false;

    public bool progressRunForTesting = false;  // TEMP

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        currentRunNumber = 1;
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void Update()
    {
        if(playerDeath)
        {
            InGameUIManager.instance.deathScreen.OpenPlayerDeathUI();
            playerDeath = false;
        }

        if(progressRunForTesting){
            progressRunForTesting = false;

            // Simulate a new run (reroll stats too)
            EndRun();
            PlayerStats pstats = FindObjectsOfType<PlayerStats>()[0];
            pstats.initializeStats();

            Debug.Log("Run Number: " + currentRunNumber);
        }
    }

    // Add any other reset stuff here too (called when player goes from death screen -> main hub)
    public void EndRun()
    {
        // Unequip all items, clear temp currency, clear potions
        PlayerInventory.instance.ClearRunInventory();

        // Set all NPC talked to variables to false and increment story beats
        StoryManager.instance.OnRunEndUpdateStory();
        StoryManager.instance.CheckForNewStoryBeats();
        
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
        currentSceneName = scene.name;

        if(currentSceneName == MAIN_HUB_STRING_NAME){
            InGameUIManager.instance.ToggleRunUI(false);

            // TODO: Play animation of you falling or something Idk
            // In particular, if currentRunNumber == 2 pause for a second before autoplaying Stellan's dialogue so that people can see the new location
        }
        else if(currentSceneName == GAME_LEVEL_STRING_NAME){
            InGameUIManager.instance.ToggleRunUI(true);
        }
    }

    public IEnumerator AutoRunDialogueAfterTime(float timeToWait = DEFAULT_AUTO_DIALOGUE_WAIT_TIME)
    {
        InputManager.instance.ToggleDialogueOpenStatus(true);   // Remove player control while waiting
        yield return new WaitForSeconds(timeToWait);
        DialogueManager.instance.OnNPCInteracted();        
    }

    public GearManagerObject GearManager()
    {
        return gearManager;
    }
}
