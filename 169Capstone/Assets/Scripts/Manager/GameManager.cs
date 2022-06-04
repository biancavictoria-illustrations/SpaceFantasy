using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // TODO: Make sure these are all correct and up to date when building
    public const string TITLE_SCREEN_STRING_NAME = "MainMenu";
    public const string MAIN_HUB_STRING_NAME = "Main Hub";
    public const string GAME_LEVEL1_STRING_NAME = "GenerationSetup";
    public const string LICH_ARENA_STRING_NAME = "Lich Fight";
    public const string EPILOGUE_SCENE_STRING_NAME = "EpilogueScene";

    private const float hitStopDuration = 0.05f;

    public string currentSceneName {get; private set;}
    public static bool generationComplete = true;

    // public const float DEFAULT_AUTO_DIALOGUE_WAIT_TIME = 1.1f;

    public int currentRunNumber {get; private set;}

    public bool hitStop {get; private set;}
    [HideInInspector] public bool deathMenuOpen;
    [HideInInspector] public bool pauseMenuOpen;
    [HideInInspector] public bool statRerollUIOpen;
    [HideInInspector] public bool inElevatorAnimation;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerTransform; // is this used for anything anywhere

    [SerializeField] private GearManagerObject gearManager;
    public JournalContentManager journalContentManager;

    [HideInInspector] public bool playerDeath = false;

    #region Gear Tier Management
        public class GearTierIncreaseEvent : UnityEvent<int> {}

        public GearTierIncreaseEvent OnTierIncreased {get; private set;}

        [HideInInspector] public int nonBossEnemiesKilledThisRun;
        public int enemiesKilledToGearTierUp = 3;
        public int gearTier {get; private set;}
    #endregion

    public GameTimer gameTimer;

    // Set to true (permanently) once you have killed the Time Lich at least once
    // (Makes a new special item pop up in Stellan's shop)
    public bool hasKilledTimeLich {get; private set;}

    // The run where the player first killed the time lich
    public int firstClearRunNumber {get; private set;}
    
    [HideInInspector] public bool epilogueTriggered = false;

    private int saveSlotNum;

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

    void Start()
    {
        AudioManager.Instance.playMusic(AudioManager.MusicTrack.TitleMusic);
        OnTierIncreased = new GearTierIncreaseEvent();
    }

    // Called when you start a new game to set values to default
    private void InitializeGameManagerValuesOnNewGame()
    {
        currentRunNumber = 1;
        hasKilledTimeLich = false;
        firstClearRunNumber = -1;

        nonBossEnemiesKilledThisRun = 0;
        gearTier = 0;
    }

    public void DocumentFirstClearInfo()
    {
        hasKilledTimeLich = true;
        firstClearRunNumber = currentRunNumber;
    }

    private void Update()
    {
        if(playerDeath)
        {
            InGameUIManager.instance.deathScreen.OpenPlayerDeathUI();
            playerDeath = false;
        }

        if(currentSceneName == TITLE_SCREEN_STRING_NAME){
            return;
        }
        else if(!DialogueManager.instance || !InGameUIManager.instance){
            Debug.LogWarning("No dialogue/UI manager found!");
            return;
        }

        if( generationComplete && GetPausedStateFromAllPauseConditions() )
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    private bool GetPausedStateFromAllPauseConditions()
    {
        return hitStop || DialogueManager.instance.stopTime || pauseMenuOpen || deathMenuOpen || InputManager.instance.shopIsOpen || InGameUIManager.instance.inventoryIsOpen || InGameUIManager.instance.gearSwapIsOpen || InputManager.instance.journalIsOpen || InputManager.instance.mapIsOpen || statRerollUIOpen;
    }

    // Add any other reset stuff here too (called when player goes from death screen -> main hub)
    public void EndRun()
    {
        // Unequip all items, clear temp currency, clear potions
        PlayerInventory.instance.ClearRunInventory();

        // Set all NPC talked to variables to false and increment story beats
        StoryManager.instance.ResetAllNPCTalkedToValues();
        StoryManager.instance.CheckForNewStoryBeats();
        
        // Increment run # -> once in main hub, run # always = # of your NEXT run (not previous)
        currentRunNumber++;

        // Reset gear tier values
        nonBossEnemiesKilledThisRun = 0;
        gearTier = 0;
    }

    public void UpdateGearTierValuesOnEnemyKilled( bool beetleBossKilled = false )
    {
        // If we're already at Legendary it's already max so just return
        if((ItemRarity)gearTier == ItemRarity.Legendary){
            return;
        }

        // If this is the beetle boss, go up a tier automatically
        if(beetleBossKilled){
            gearTier++;
            OnTierIncreased.Invoke(gearTier);
            return;
        }

        nonBossEnemiesKilledThisRun++;
        InGameUIManager.instance.lootTierUI.IncrementTierUI();

        if( nonBossEnemiesKilledThisRun % enemiesKilledToGearTierUp == 0 && (ItemRarity)gearTier < ItemRarity.Legendary ){
            gearTier++;
            OnTierIncreased.Invoke(gearTier);
        }
        // check how gear tiers are supposed to work
        // in some places (like shops (already dealt with, maybe correctly), MAYBE enemy drops) things might get weird if it drops tier and tier+1 cuz legendary+1 would be enumSize
    }

    #region Scene Load Stuff
        void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            currentSceneName = scene.name;

            AudioManager.Instance.stopMusic(true);

            ScreenFade fade = FindObjectOfType<ScreenFade>();

            // === MAIN HUB ===
            if(currentSceneName == MAIN_HUB_STRING_NAME){
                InGameUIManager.instance.ToggleRunUI(false, false, true, false);

                fade.opaqueOnStart = true;
                fade.FadeIn(0.5f);

                // TODO: play main hub music

                // TODO: Play animation of you falling or something Idk (or like phasing in)

                // Autosave in main hub!
                SaveGame();
            }

            // === LEVEL ===
            else if(currentSceneName == GAME_LEVEL1_STRING_NAME){
                PlayerInventory.instance.SetRunStartHealthPotionQuantity();            
                // AudioManager.Instance.playMusic(AudioManager.MusicTrack.Level1, false);

                InGameUIManager.instance.gameObject.SetActive(false);   // Activated in RemoveLoadScreen()
                
                FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener( () =>
                {
                    fade.SetOpaque();
                    LoadScreen.instance.RemoveLoadScreen();
                    fade.FadeIn(1f);

                    // For an average run (beyond run # 1)
                    if(currentRunNumber != 1){
                        InGameUIManager.instance.ToggleRunUI(false, false, true, false);
                        InGameUIManager.instance.TogglePermanentCurrencyUI(false);

                        inElevatorAnimation = true;
                        FindObjectOfType<ElevatorAnimationHelper>().AddListenerToAnimationEnd( () => {
                            InGameUIManager.instance.EnableRunStartStatRerollPopup(true);
                            inElevatorAnimation = false;
                        });
                    }
                    // If first run, skip reroll
                    else{
                        InGameUIManager.instance.ToggleRunUI(true, true, true, false);
                    }
                });
            }

            // === LICH FIGHT ROOM ===
            else if(currentSceneName == LICH_ARENA_STRING_NAME){
                // TODO: Play lich music

                inElevatorAnimation = true;
                FindObjectOfType<ElevatorAnimationHelper>().AddListenerToAnimationEnd( () => {
                    inElevatorAnimation = false;

                    // Set all the UI active
                    ReactivateUIAfterElevatorAnimation(true);
                });

                OnSceneLoadIfPlayerNotDestroyed(fade);
            }

            // === EPILOGUE HUB ===
            else if(currentSceneName == EPILOGUE_SCENE_STRING_NAME){
                StoryManager.instance.ResetAllNPCTalkedToValues();

                // TODO: Play end music?

                inElevatorAnimation = true;
                FindObjectOfType<ElevatorAnimationHelper>().AddListenerToAnimationEnd( () => {
                    // Trigger epilogue intro dialogue
                    StartCoroutine(DialogueManager.instance.AutoRunDialogueAfterTime());

                    inElevatorAnimation = false;

                    // Set all the UI active
                    ReactivateUIAfterElevatorAnimation(false);
                });

                OnSceneLoadIfPlayerNotDestroyed(fade);
            }

            // === TITLE SCREEN ===
            else if(currentSceneName == TITLE_SCREEN_STRING_NAME){
                gameTimer.runTotalTimer = false;
                AudioManager.Instance.playMusic(AudioManager.MusicTrack.TitleMusic);

                // Reset values that we don't want to carry over, even if you continue a save file
                StoryManager sm = StoryManager.instance;
                sm.talkedToBryn = false;
                sm.talkedToDoctor = false;
                sm.talkedToRhian = false;
                sm.talkedToLich = false;

                DialogueManager dm = DialogueManager.instance;
                dm.SetStellanCommTriggered(false);
                dm.SetTimeLichDeathDialogueTriggered(false);
                dm.SetCaptainsLogDialogueTriggered(false);
            }
        }

        private void OnSceneLoadIfPlayerNotDestroyed(ScreenFade fade)
        {
            fade.opaqueOnStart = true;
            fade.FadeIn(0.5f);
            
            UnparentPlayerOnSceneLoad();
            InGameUIManager.instance.TogglePermanentCurrencyUI(false);
            InGameUIManager.instance.ToggleRunUI(false, false, false, false);
        }

        private void ReactivateUIAfterElevatorAnimation(bool setTimerUIActive)
        {
            InGameUIManager.instance.SetAllRunUIToCurrentValues();
            InGameUIManager.instance.TogglePermanentCurrencyUI(true);
            InGameUIManager.instance.ToggleRunUI(true, setTimerUIActive, false, false);
        }

        private void UnparentPlayerOnSceneLoad()
        {
            // Make the player no longer a child of the game manager now that we've saved their build between scenes
            // Move the player out of dontdestroyonload
            GameObject o = new GameObject();
            Player.instance.transform.parent = o.transform;
            
            // Unparent it
            Player.instance.transform.parent = null;

            // Destroy the dummy game object
            Destroy(o.gameObject);
        }
    
        public bool InSceneWithRandomGeneration()
        {
            return currentSceneName == GAME_LEVEL1_STRING_NAME;
        }

        public bool InSceneWithGameTimer()
        {
            return GameManager.instance.currentSceneName != GameManager.MAIN_HUB_STRING_NAME && GameManager.instance.currentSceneName != GameManager.EPILOGUE_SCENE_STRING_NAME;
        }
    #endregion

    public GearManagerObject GearManager()
    {
        return gearManager;
    }

    #region Hit Stop
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
    #endregion

    #region Save/Load
        public void SaveGame()
        {
            SaveDisplayValuesToPlayerPrefs();
            MarkSlotToDelete(false, saveSlotNum);
            
            SaveLoadManager.SaveGame(saveSlotNum, this, PlayerInventory.instance, DialogueManager.instance, StoryManager.instance, PermanentUpgradeManager.instance, journalContentManager);        
        }

        // Called when you load your game in the Main Menu (and ONLY then)
        public void LoadGame(int _slot)
        {
            saveSlotNum = _slot;

            gameTimer.SetTotalTimeOnThisSaveFile( GetTotalTimePlayedInSaveFile(_slot) );

            // Retrieve save data & set all values from there
            Save saveData = SaveLoadManager.LoadGame(saveSlotNum);

            // Game Manager Stuff
            currentRunNumber = saveData.currentRunNumber;
            hasKilledTimeLich = saveData.hasKilledTimeLich;
            firstClearRunNumber = saveData.firstClearRunNumber;

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
            sm.lichListInitialized = saveData.lichListInitialized;

            for(int i = 0; i < saveData.brynNumRunDialogueList.Length; i++){
                sm.brynNumRunDialogueList.Add(saveData.brynNumRunDialogueList[i]);
            }
            for(int i = 0; i < saveData.stellanNumRunDialogueList.Length; i++){
                sm.stellanNumRunDialogueList.Add(saveData.stellanNumRunDialogueList[i]);
            }
            for(int i = 0; i < saveData.timeLichNumRunDialogueList.Length; i++){
                sm.timeLichNumRunDialogueList.Add(saveData.timeLichNumRunDialogueList[i]);
            }

            // Story Beat Status Values
            sm.LoadSavedStoryBeatStatuses(saveData.storyBeatDatabaseStatuses, saveData.itemStoryBeatStatuses, saveData.genericStoryBeatStatuses, saveData.activeStoryBeatHeadNodes);

            // Journal status
            journalContentManager.SetJournalStatusOnLoad( saveData.journalContentSaveStatus );

            // Load scene after fade to black
            ScreenFade fade = FindObjectOfType<ScreenFade>();
            fade.AddListenerToFadeEnd( () => {
                SceneManager.LoadScene(GameManager.MAIN_HUB_STRING_NAME);
            });
            fade.FadeOut(0.5f);

            gameTimer.runTotalTimer = true;
        }

        public void StartNewGame(int _newSlot)
        {
            saveSlotNum = _newSlot;
            
            // Save that we are using this save file now
            PlayerPrefs.SetInt(GetSaveFilePlayerPrefsKey(saveSlotNum), 1);
            PlayerPrefs.Save();

            // If starting a new game, load level 1 scene (new game) after fade to black
            ScreenFade fade = FindObjectOfType<ScreenFade>();
            fade.AddListenerToFadeEnd( () => {
                SceneManager.LoadScene(GameManager.GAME_LEVEL1_STRING_NAME);
            });
            fade.FadeOut(1f);

            // Reset all starting values for anything set to dontdestroyonload
            InitializeGameManagerValuesOnNewGame();
            PlayerInventory.instance.InitializeInventoryValuesOnNewGame();
            PermanentUpgradeManager.instance.InitializePermanentUpgradeValuesOnNewGame();
            StoryManager.instance.InitializeStoryManagerOnNewGame();

            DialogueManager.instance.visitedNodes.Clear();

            journalContentManager.ReloadDefaultJournalStatus();

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

            PlayerPrefs.SetFloat( s + "TimePlayed", gameTimer.totalTimePlayedOnThisSaveFile );
            
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
                Debug.LogWarning("No value found for key " + s + " in PlayerPrefs; okay if checking for completed runs > 0");
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

        public string GetPlayerPrefsMarkedToDeleteFileKey(int _slot)
        {
            return GameManager.instance.GetSaveFilePlayerPrefsKey(_slot) + "Delete";
        }

        public void MarkSlotToDelete(bool set, int _slot)
        {
            // Save it to prefs so that we remember between play sessions in case it's not overwritten right now
            if(set){
                PlayerPrefs.SetInt(GetPlayerPrefsMarkedToDeleteFileKey(_slot), 1);
            }
            else{
                PlayerPrefs.SetInt(GetPlayerPrefsMarkedToDeleteFileKey(_slot), 0);            
            }
            PlayerPrefs.Save();
        }
    #endregion
}
