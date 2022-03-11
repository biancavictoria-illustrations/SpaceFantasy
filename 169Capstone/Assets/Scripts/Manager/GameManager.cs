using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public const string TITLE_SCREEN_STRING_NAME = "MainMenu";
    public const string MAIN_HUB_STRING_NAME = "Main Hub";
    public const string GAME_LEVEL1_STRING_NAME = "GenerationSetup";
    public const string LICH_ARENA_STRING_NAME = "LichArena";   // TODO: Update this!!!

    private const float hitStopDuration = 0.1f;

    public string currentSceneName {get; private set;}

    public const float DEFAULT_AUTO_DIALOGUE_WAIT_TIME = 1.1f;

    public int currentRunNumber {get; private set;}

    public bool hitStop {get; private set;}
    [HideInInspector] public bool deathMenuOpen;
    [HideInInspector] public bool pauseMenuOpen;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerTransform; // TODO: set this at runtime if game manager starts in main menu???

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

        if(currentSceneName == TITLE_SCREEN_STRING_NAME){
            return;
        }
        else if(!DialogueManager.instance || !InGameUIManager.instance){
            Debug.LogError("No dialogue/UI manager found!");
        }

        if(hitStop || DialogueManager.instance.stopTime || pauseMenuOpen || deathMenuOpen || InputManager.instance.shopIsOpen || InGameUIManager.instance.inventoryIsOpen || InGameUIManager.instance.gearSwapIsOpen)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
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

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;

        AudioManager.Instance.stopMusic(true);

        if(currentSceneName == MAIN_HUB_STRING_NAME){
            InGameUIManager.instance.ToggleRunUI(false);

            // TODO: play main hub music

            // TODO: Play animation of you falling or something Idk (or like phasing in)

            SaveLoadManager.SaveGame(this, PlayerInventory.instance, DialogueManager.instance, StoryManager.instance, PermanentUpgradeManager.instance);    // Autosave in main hub!
        }
        else if(currentSceneName == GAME_LEVEL1_STRING_NAME){
            PlayerInventory.instance.SetRunStartHealthPotionQuantity();
            InGameUIManager.instance.ToggleRunUI(true);
            AudioManager.Instance.playMusic(AudioManager.MusicTrack.Level1, false);
        }
        else if(currentSceneName == LICH_ARENA_STRING_NAME){
            // TODO: Play lich fight music
            
        }
        else if(currentSceneName == TITLE_SCREEN_STRING_NAME){
            // TODO: Play title screen music

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

    public void EnableHitStop()
    {
        hitStop = true;
        StartCoroutine(DisableHitStop());
    }

    private IEnumerator DisableHitStop()
    {
        yield return new WaitForSecondsRealtime(hitStopDuration);
        hitStop = false;
    }

    // Called when you load your game in the Main Menu
    public void LoadGame()
    {
        Save saveData = SaveLoadManager.LoadGame();

        // Set all values
    }
}
