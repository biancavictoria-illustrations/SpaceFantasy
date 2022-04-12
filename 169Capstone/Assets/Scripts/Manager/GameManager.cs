using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // TODO: Make sure these are all correct and up to date
    public const string TITLE_SCREEN_STRING_NAME = "MainMenu";
    public const string MAIN_HUB_STRING_NAME = "Main Hub";
    public const string GAME_LEVEL1_STRING_NAME = "GenerationSetup";
    public const string LICH_ARENA_STRING_NAME = "LichArena";

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
    [HideInInspector] public int bossesKilled;

    public GameTimer gameTimer;

    // Set to true (permanently) once you have killed the Time Lich at least once
    // (Makes a new special item pop up in Stellan's shop)
    [HideInInspector] public bool hasKilledTimeLich;

    private int saveSlotNum;

    public bool progressRunForTesting = false;  // TEMP

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
            return;
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);        
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    // Called when you start a new game to set values to default
    private void InitializeGameManagerValuesOnNewGame()
    {
        currentRunNumber = 1;
        hasKilledTimeLich = false;
        bossesKilled = 0;
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

            // Autosave in main hub!
            SaveGame();
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
            gameTimer.runTotalTimer = false;

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


    #region Save/Load

    public void SaveGame()
    {
        SaveDisplayValuesToPlayerPrefs();
        SaveLoadManager.SaveGame(saveSlotNum, this, PlayerInventory.instance, DialogueManager.instance, StoryManager.instance, PermanentUpgradeManager.instance);
    }

    // Called when you load your game in the Main Menu (and ONLY then)
    public void LoadGame(int _slot)
    {
        saveSlotNum = _slot;
        Debug.Log("Loading Game... Save Slot Num in Game Manager set to: " + saveSlotNum);

        // Retrieve save data & set all values from there
        Save saveData = SaveLoadManager.LoadGame(saveSlotNum);

        // Game Manager Stuff
        currentRunNumber = saveData.currentRunNumber;
        hasKilledTimeLich = saveData.hasKilledTimeLich;
        bossesKilled = saveData.bossesKilled;

        // Inventory Stuff
        PlayerInventory.instance.SetPermanentCurrency(saveData.permanentCurrency, false);

        // Permanent Upgrades (Skills & Stats)
        PermanentUpgradeManager pum = PermanentUpgradeManager.instance;

        pum.totalPermanentCurrencySpent = saveData.totalPermanentCurrencySpent;
        
        pum.startingHealthPotionQuantity = saveData.startingHealthPotionQuantity;
        pum.levelsInArmorPlating = saveData.levelsInArmorPlating;
        pum.levelsInExtensiveTraining = saveData.levelsInExtensiveTraining;
        pum.levelsInNatural20 = saveData.levelsInNatural20;
        pum.levelsInPrecisionDrive = saveData.levelsInPrecisionDrive;
        pum.levelsInTimeLichKillerThing = saveData.levelsInTimeLichKillerThing;

        pum.strMin = saveData.strMin;
        pum.strMax = saveData.strMax;
        pum.dexMin = saveData.dexMin;
        pum.dexMax = saveData.dexMax;
        pum.intMin = saveData.intMin;
        pum.intMax = saveData.intMax;
        pum.wisMin = saveData.wisMin;
        pum.wisMax = saveData.wisMax;
        pum.conMin = saveData.conMin;
        pum.conMax = saveData.conMax;
        pum.charismaMin = saveData.charismaMin;
        pum.charismaMax = saveData.charismaMax;

        // Dialogue System Stuff
        for(int i = 0; i < saveData.visistedNodes.Length; i++){
            DialogueManager.instance.visitedNodes.Add(saveData.visistedNodes[i]);
        }

        StoryManager sm = StoryManager.instance;

        sm.talkedToStellan = saveData.talkedToStellan;

        sm.brynListInitialized = saveData.brynListInitialized;
        sm.stellanListInitialized = saveData.stellanListInitialized;
        sm.doctorListInitialized = saveData.doctorListInitialized;
        sm.lichListInitialized = saveData.lichListInitialized;
        sm.rhianListInitialized = saveData.rhianListInitialized;

        for(int i = 0; i < saveData.brynNumRunDialogueList.Length; i++){
            sm.brynNumRunDialogueList.Add(saveData.brynNumRunDialogueList[i]);
        }
        for(int i = 0; i < saveData.stellanNumRunDialogueList.Length; i++){
            sm.stellanNumRunDialogueList.Add(saveData.stellanNumRunDialogueList[i]);
        }
        for(int i = 0; i < saveData.doctorNumRunDialogueList.Length; i++){
            sm.doctorNumRunDialogueList.Add(saveData.doctorNumRunDialogueList[i]);
        }
        for(int i = 0; i < saveData.timeLichNumRunDialogueList.Length; i++){
            sm.timeLichNumRunDialogueList.Add(saveData.timeLichNumRunDialogueList[i]);
        }
        for(int i = 0; i < saveData.rhianNumRunDialogueList.Length; i++){
            sm.rhianNumRunDialogueList.Add(saveData.rhianNumRunDialogueList[i]);
        }

        // Story Beat Status Values
        sm.LoadSavedStoryBeatStatuses(saveData.storyBeatDatabaseStatuses, saveData.itemStoryBeatStatuses, saveData.genericStoryBeatStatuses, saveData.activeStoryBeatHeadNodes);

        SceneManager.LoadScene(GameManager.MAIN_HUB_STRING_NAME);
        gameTimer.runTotalTimer = true;
    }

    public void StartNewGame(int _newSlot)
    {
        saveSlotNum = _newSlot;
        
        // Save that we are using this save file now
        PlayerPrefs.SetInt(GetSaveFilePlayerPrefsKey(saveSlotNum), 1);
        PlayerPrefs.Save();

        // If starting a new game, load level 1 scene (new game)
        SceneManager.LoadScene(GameManager.GAME_LEVEL1_STRING_NAME);

        // Reset all starting values for anything set to dontdestroyonload
        InitializeGameManagerValuesOnNewGame();
        PlayerInventory.instance.InitializeInventoryValuesOnNewGame();
        PermanentUpgradeManager.instance.InitializePermanentUpgradeValuesOnNewGame();
        StoryManager.instance.InitializeStoryManagerOnNewGame();

        DialogueManager.instance.visitedNodes.Clear();

        gameTimer.ResetTotalTimer();
        gameTimer.runTotalTimer = true;
    }

    public void DeleteSaveFile(int _slot)
    {
        PlayerPrefs.SetInt(GetSaveFilePlayerPrefsKey(_slot), 0);
        PlayerPrefs.Save();
    }

    public string GetSaveFilePlayerPrefsKey(int _slot)
    {
        return "SaveFile" + _slot;
    }

    public bool SaveFileIsFull(int _slot)
    {
        string s = GetSaveFilePlayerPrefsKey(_slot);

        // If the key is not even in player prefs, we have never set a save file for that value
        if(!PlayerPrefs.HasKey(s)){
            return false;
        }
        // If the key IS there and the value is 1, there is currently a save file there
        else if(PlayerPrefs.GetInt(s) == 1){
            return true;
        }
        // Else if 0 or -1, there's no save file there currently
        return false;
    }

    private void SaveDisplayValuesToPlayerPrefs()
    {
        string s = GetSaveFilePlayerPrefsKey(saveSlotNum);

        PlayerPrefs.SetInt( s + "StarShards", PermanentUpgradeManager.instance.totalPermanentCurrencySpent + PlayerInventory.instance.permanentCurrency );
        
        // If we're calling this ONLY in Main Hub, it's current run num - 1
        PlayerPrefs.SetInt( s + "CompletedRunNum", currentRunNumber - 1 );

        // If total time doesn't exist yet for this save file, set it for the first time; else, update it
        float totalTimeFromPlayerPrefs = GetTotalTimePlayedInSaveFile(saveSlotNum);
        if(totalTimeFromPlayerPrefs != -1){
            PlayerPrefs.SetFloat( s + "TimePlayed", gameTimer.totalTimePlayedOnThisSaveFile + totalTimeFromPlayerPrefs );
        }
        else{
            PlayerPrefs.SetFloat( s + "TimePlayed", gameTimer.totalTimePlayedOnThisSaveFile );
        }
        
        PlayerPrefs.Save();
    }

    public int GetStarShardsCollectedInSaveFile(int _slot)
    {
        string s = GetSaveFilePlayerPrefsKey(_slot) + "StarShards";
        if(!PlayerPrefs.HasKey(s)){
            Debug.LogError("No value found for key " + s + " in PlayerPrefs");
            return -1;
        }
        return PlayerPrefs.GetInt(s);
    }

    public int GetNumCompletedRunsInSaveFile(int _slot)
    {
        string s = GetSaveFilePlayerPrefsKey(_slot) + "CompletedRunNum";
        if(!PlayerPrefs.HasKey(s)){
            Debug.LogError("No value found for key " + s + " in PlayerPrefs");
            return -1;
        }
        return PlayerPrefs.GetInt(s);
    }

    public float GetTotalTimePlayedInSaveFile(int _slot)
    {
        string s = GetSaveFilePlayerPrefsKey(_slot) + "TimePlayed";
        if(!PlayerPrefs.HasKey(s)){
            Debug.LogWarning("No value found for key " + s + " in PlayerPrefs; okay if checking in SaveDisplayValuesToPlayerPrefs");
            return -1;
        }
        return PlayerPrefs.GetFloat(s);
    }

    #endregion
}
