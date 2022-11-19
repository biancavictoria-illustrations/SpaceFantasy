using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;


public class DialogueManager : MonoBehaviour
{   
    public static DialogueManager instance;

    public GameObject dialogueUICanvas;
    public DialogueUI dialogueUI;

    public Button nextButton;

    [SerializeField] private List<SpeakerData> allSpeakersInGame = new List<SpeakerData>();    // For setting in the inspector and building the dictionary at runtime
    private Dictionary<string, SpeakerData> speakers = new Dictionary<string, SpeakerData>();

    public Image characterPortrait; 
    public TMP_Text speakerName;

    #region Emote Holders
        [SerializeField] private List<DialogueEmote> atlanEmoteHolder = new List<DialogueEmote>();
        [SerializeField] private List<DialogueEmote> stellanEmoteHolder = new List<DialogueEmote>();
        [SerializeField] private List<DialogueEmote> brynEmoteHolder = new List<DialogueEmote>();
        [SerializeField] private List<DialogueEmote> doctorEmoteHolder = new List<DialogueEmote>();
        [SerializeField] private List<DialogueEmote> rhianEmoteHolder = new List<DialogueEmote>();
        [SerializeField] private List<DialogueEmote> timeLichEmoteHolder = new List<DialogueEmote>();
    #endregion

    public DialogueRunner dialogueRunner;

    public HashSet<string> visitedNodes = new HashSet<string>();       // Keeps track of what nodes the player has seen so that we don't see those again

    [SerializeField] private int numRunsThreshold = 4;   // Threshold for # runs beyond the exact num run that numRun dialogue can trigger

    #region Auto Dialogue Triggers
        public bool stellanCommTriggered {get; private set;}
        public bool timeLichDeathDialogueTriggered {get; private set;}
        public bool captainsLogDialogueTriggered {get; private set;}
    #endregion

    private bool hasBeenInitialized = false;

    [HideInInspector] public bool stopTime;

    #region Const Setting Values
        public const float DEFAULT_TEXT_SPEED = 0.015f;
        public const float FAST_TEXT_SPEED = 0.008f;

        public const float DEFAULT_AUTO_DIALOGUE_WAIT_TIME = 1.1f;
    #endregion

    void Awake()
    {
        // Make this a singleton so that it can be accessed from anywhere and there's only one
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }

        if(!hasBeenInitialized){
            SetYarnFunctions();
            hasBeenInitialized = true;
        }

        foreach(SpeakerData s in allSpeakersInGame){
            if( dialogueRunner.yarnScripts.Length == 0 ){
                dialogueRunner.Add(s.YarnDialogue());
            }
            else if(!dialogueRunner.NodeExists(s.GetYarnHeadNode())){
                dialogueRunner.Add(s.YarnDialogue());
            }

            if(!DialogueManagerHasSpeaker(s)){
                AddSpeakerToDictionary(s);
            }
        }

        stellanCommTriggered = false;
    }

    #region No Active NPC Trigger Setters
        public void SetStellanCommTriggered(bool set)
        {
            stellanCommTriggered = set;
        }

        public void SetTimeLichDeathDialogueTriggered(bool set)
        {
            timeLichDeathDialogueTriggered = set;
        }

        public void SetCaptainsLogDialogueTriggered(bool set)
        {
            captainsLogDialogueTriggered = set;
        }
    #endregion

    public void SetTextSpeed(float number)
    {
        dialogueUI.textSpeed = number;
    }

    private void SetYarnFunctions()
    {
        // Add commands to Yarn so that we can send Unity info from there
        dialogueRunner.AddCommandHandler("SetSpeaker", SetSpeakerUI);
        dialogueRunner.AddCommandHandler("Emote", AddEmoteForSpeaker);
        dialogueRunner.AddCommandHandler("BranchComplete", BranchComplete);

        // Add Visited to yarn so that we can check if a node has been visited yet
        dialogueRunner.AddFunction("Visited", 1, delegate (Yarn.Value[] parameters){
            var nodeName = parameters[0];
            return visitedNodes.Contains(nodeName.AsString);
        });

        // Add random number generator for repeatable and other options that can be played out of order
        // Min 0, pass in Max (non-inclusive)
        dialogueRunner.AddFunction("RandomNum", 1, delegate (Yarn.Value[] parameters){
            var numMaxExclusive = parameters[0];
            return Random.Range(0, (int)numMaxExclusive.AsNumber);
        });

        // Add num run checker to retrieve the number of runs you have COMPLETED (so current num - 1)
        dialogueRunner.AddFunction("GetNumberOfCompletedRuns", 0, delegate (Yarn.Value[] parameters){
            return GameManager.instance.currentRunNumber - 1;
        });

        // Add CURRENT num run checker
        dialogueRunner.AddFunction("GetCurrentRunNum", 0, delegate (Yarn.Value[] parameters){
            return GameManager.instance.currentRunNumber;
        });

        dialogueRunner.AddFunction("StellanCommTriggered", 0, delegate (Yarn.Value[] parameters){
            if(DialogueManager.instance.stellanCommTriggered){
                DialogueManager.instance.SetStellanCommTriggered(false);
                return true;
            }
            else{
                return false;
            }
        });

        dialogueRunner.AddFunction("TimeLichDeathDialogueTriggered", 0, delegate (Yarn.Value[] parameters){
            if(DialogueManager.instance.timeLichDeathDialogueTriggered){
                DialogueManager.instance.SetTimeLichDeathDialogueTriggered(false);
                return true;
            }
            else{
                return false;
            }
        });

        dialogueRunner.AddFunction("CaptainsLogDialogueTriggered", 0, delegate (Yarn.Value[] parameters){
            if(DialogueManager.instance.captainsLogDialogueTriggered){
                DialogueManager.instance.SetCaptainsLogDialogueTriggered(false);
                return true;
            }
            else{
                return false;
            }
        });

        dialogueRunner.AddFunction("EpilogueTriggered", 0, delegate (Yarn.Value[] parameters){
            return GameManager.instance.epilogueTriggered;
        });

        dialogueRunner.AddFunction("HasDeusExMachina", 0, delegate (Yarn.Value[] parameters){
            return PermanentUpgradeManager.instance.GetSkillLevel(PermanentUpgradeType.TimeLichKillerThing) > 0;
        });

        dialogueRunner.AddFunction("HasKilledTimeLich", 0, delegate (Yarn.Value[] parameters){
            return GameManager.instance.hasKilledTimeLich;
        });

        dialogueRunner.AddFunction("FirstClearRunNumber", 0, delegate (Yarn.Value[] parameters){
            return GameManager.instance.firstClearRunNumber;
        });

        // Add SelectNextTrigger so that we can find the next conditional category of dialogue to play
        dialogueRunner.AddFunction("SelectNextNode", 0, delegate (Yarn.Value[] parameters){
            // Sorted database of all beats available at the moment (all for which conditions are met)
            SortedSet<StoryBeat> sortedStoryBeats = new SortedSet<StoryBeat>(new CompareBeatsByPriority());
            
            // Check for Killed Events and Conversation Events
            if( StoryManager.instance.activeStoryBeats.Count > 0 ){
                // Cycle the options and add ones for which the active NPC has something to say
                foreach(StoryBeat beat in StoryManager.instance.activeStoryBeats){
                    if( StoryManager.instance.SpeakerIsInSpeakerList(beat, NPC.ActiveNPC.SpeakerData().SpeakerID()) ){
                        sortedStoryBeats.Add(beat);
                    }
                }
            }

            // Check for item event triggers and if any are met add them to the pool
            foreach( Equipment item in PlayerInventory.instance.gear.Values ){
                if(!item){
                    continue;
                }

                // Check if this item has a story beat associated with it
                string nodeName = StoryBeatType.Item + item.data.equipmentBaseData.ItemID().ToString();
                StoryBeat itemStoryBeat = StoryManager.instance.FindBeatFromNodeNameAndType( nodeName, StoryBeatType.Item );
                
                // If there isn't a story beat associated with this item OR this specific NPC doesn't have anything to say about it, just move on
                if(!itemStoryBeat || !StoryManager.instance.SpeakerIsInSpeakerList(itemStoryBeat, NPC.ActiveNPC.SpeakerData().SpeakerID())){
                    continue;
                }

                // If we've reached this point, add it to the pool of dialogue options
                sortedStoryBeats.Add(itemStoryBeat);
                Debug.Log("Added ITEM STORY BEAT " + nodeName + " to dialogue pool");
            }

            // For bargain success & fail interactions
            PlayerStats playerStats = FindObjectsOfType<PlayerStats>()[0];
            
            // If we see a numRun dialogue interaction, we need to be able to remove that option from the pool later
            int currentNumRunRemoveValue = 0;   

            // Loop through genericDialogueList and check if the conditions are met for each trigger type, then add if necessary
            foreach( StoryBeat beat in StoryManager.instance.genericStoryBeats.Keys ){
                // Confirm that the active NPC has something to say about this beat; if not, skip it and move on to the next option
                if( !StoryManager.instance.SpeakerIsInSpeakerList(beat, NPC.ActiveNPC.SpeakerData().SpeakerID()) ){
                    continue;
                }

                StoryBeatType beatType = beat.GetBeatType();

                // If beat type is repeatable, add it regardless of conditions
                if( beatType == StoryBeatType.Repeatable ){
                    sortedStoryBeats.Add(beat);
                }
                // If beat type is default, add it as long as this NPC has something to say
                else if( beatType == StoryBeatType.DefaultDialogue ){
                    sortedStoryBeats.Add(beat);
                }
                // If the beat type is numRuns, check conditions and add it to the pool if necessary
                else if( beatType == StoryBeatType.NumRuns && NPC.ActiveNPC ){
                    // Separate from currentNumRunRemoveValue, if one has passed the threshold in which we're okay with seeing runNum dialogue, remove it
                    int opportunityPassedRemoveValue = 0;
                    int currentRunNumber = GameManager.instance.currentRunNumber;
                    foreach(int num in NPC.ActiveNPC.GetNumRunDialogueList()){
                        int difference = currentRunNumber - num;
                        // If THIS NPC has something to say about your number of runs within threshold runs, add it to the pool
                        if( difference <= numRunsThreshold && difference >= 0 && !visitedNodes.Contains(NPC.ActiveNPC.SpeakerData().SpeakerID() + beat.GetYarnHeadNode()) ){
                            sortedStoryBeats.Add(beat);
                            currentNumRunRemoveValue = num;
                        }
                        else if( difference > numRunsThreshold ){
                            // If we've passed the threshold, remove it from the list so we stop looking for it
                            opportunityPassedRemoveValue = num;
                        }
                    }
                    NPC.ActiveNPC.GetNumRunDialogueList().Remove(opportunityPassedRemoveValue); // I don't think this saves so there's kinda no point to this...? Not sure
                }
                // If the beat type is lowHP, check conditions and maybe add it
                else if( beatType == StoryBeatType.LowHealth ){
                    StoryBeatLowHealth lowHPBeat = (StoryBeatLowHealth)beat;
                    if( Player.instance.health.currentHitpoints / Player.instance.health.maxHitpoints <= lowHPBeat.LowHealthThreshold() ){
                        sortedStoryBeats.Add(beat);
                    }
                }
                // If beat type is barter, check conditions and maybe add it
                else if( beatType == StoryBeatType.BarterFail || beatType == StoryBeatType.BarterSuccess ){
                    if( (beatType == StoryBeatType.BarterSuccess && playerStats.Charisma() >= 15) || (beatType == StoryBeatType.BarterFail && playerStats.Charisma() < 10) ){
                        sortedStoryBeats.Add(beat);
                    }
                }
                else{
                    Debug.LogError("Found beat of type " + beatType + " in StoryManager.instance.genericStoryBeats.");
                }
            }

            // Debug.Log("PLAYING DIALOGUE INTERACTION for " + NPC.ActiveNPC.SpeakerData().SpeakerID() + ": " + sortedStoryBeats.Max.GetYarnHeadNode());

            // Return the highest priority beat
            return sortedStoryBeats.Max.GetYarnHeadNode();
        });

        dialogueRunner.AddFunction("Stat", 1, delegate (Yarn.Value[] parameters){
            string statValue = parameters[0].AsString;
            switch(statValue){
                case "STR":
                    return Player.instance.stats.Strength();
                case "DEX":
                    return Player.instance.stats.Dexterity();
                case "CON":
                    return Player.instance.stats.Constitution();
                case "INT":
                    return Player.instance.stats.Intelligence();
                case "WIS":
                    return Player.instance.stats.Wisdom();
            }
            Debug.LogError("No stat value found for stat code: " + statValue);
            return -1;
        });

        // I don't think this is being used anywhere...
        // Add a function to tell yarn which node in that conditional branch to play
        dialogueRunner.AddFunction("SelectGenericNode", 1, delegate (Yarn.Value[] parameters){
            // Takes in a trigger; search the active NPC's dictionary for that key, the value is an int of where we're at
            StoryBeatType trigger = StoryBeatType.DefaultDialogue;
            for(int i = 0; i < (int)StoryBeatType.enumSize; ++i){
                if(( (StoryBeatType)i ).ToString().Equals(parameters[0])){
                    trigger = (StoryBeatType)i;
                }
            }
            return 0;
        });
    }

    #region Speaker Management
        public bool DialogueManagerHasSpeaker(SpeakerData data)
        {
            if(speakers.ContainsKey(data.SpeakerID().ToString())){
                return true;
            }
            return false;
        }

        public void AddSpeakerToDictionary(SpeakerData data)
        {
            if(DialogueManagerHasSpeaker(data)){
                Debug.LogError("Attempting to add " + data.SpeakerName() + " to the speaker database, but it already exists!");
                return;
            }
            speakers.Add(data.SpeakerID().ToString(), data);
        }
    #endregion

    #region Dialogue UI Setters (For Yarn)
        // Called in yarn scripts to set UI speaker info
        private void SetSpeakerUI(string[] info)
        {
            // Set speaker name
            string speaker = info[0];
            string speakerNameOverride = "";

            // Set portrait
            string portrait = "";
            if(info.Length == 1){   // ONE parameter (speakerID)
                portrait = SpeakerData.PORTRAIT_NEUTRAL;            
            }
            else if(info.Length == 2){  // TWO parameters (speakerID, emotion)
                portrait = info[1];
            }
            else{  // THREE parameters (speakerID, emotion, speaker display name override)
                portrait = info[1];
                speakerNameOverride = info[2];
            }
            
            if(speakers.TryGetValue(speaker, out SpeakerData data)){
                characterPortrait.sprite = data.GetEmotionPortrait(portrait);

                if(speakerNameOverride == ""){
                    speakerName.text = data.SpeakerName();
                }
                else{
                    speakerName.text = speakerNameOverride;
                }
                
                return;
            }
            else{   // If no SpeakerData is found...
                Debug.LogError("Could not set the speaker data for " + speaker);
            }
        }

        /* In yarn files, call AddEmoteForSpeaker like:
            
        <<Emote Stellan question>>
        <<Emote Player surprise>>
            
        */
        private void AddEmoteForSpeaker(string[] info)
        {
            // First parameter: SpeakerID
            string speakerString = info[0];

            // Second parameter: DialogueEmoteType
            string emoteString = info[1];

            if(speakers.TryGetValue(speakerString, out SpeakerData data)){
                // Get the correlated emote holder object in the scene
                List<DialogueEmote> emoteHolder = GetEmoteHolderFromSpeakerID(data.SpeakerID());

                // Get the EmoteType enum value we're looking for
                DialogueEmoteType emoteType = GetEmoteTypeFromString(emoteString);

                // Activate the corresponding emote
                foreach(DialogueEmote e in emoteHolder){
                    if( e.EmoteType() == emoteType ){
                        e.ToggleEmoteActive(true);
                        return;
                    }
                }

                // If we haven't returned yet, throw an error cuz nothing matching was found
                Debug.LogError("No emote '" + emoteString + "' with ID " + emoteType + " found for speaker " + speakerString);
            }
            else{
                Debug.LogError("Failed to find speaker " + speakerString + " to set emote " + emoteString);
            }
        }

        // Helper
        private List<DialogueEmote> GetEmoteHolderFromSpeakerID(SpeakerID speaker)
        {
            switch(speaker){
                case SpeakerID.Player:
                    return atlanEmoteHolder;
                case SpeakerID.Stellan:
                    return stellanEmoteHolder;
                case SpeakerID.Bryn:
                    return brynEmoteHolder;
                case SpeakerID.Rhian:
                    return rhianEmoteHolder;
                case SpeakerID.Doctor:
                    return doctorEmoteHolder;
                case SpeakerID.TimeLich:
                    return timeLichEmoteHolder;
            }
            Debug.LogError("No emote holder found for speaker: " + speaker);
            return null;
        }

        // Helper
        private DialogueEmoteType GetEmoteTypeFromString(string value)
        {
            for(int i = 0; i < (int)DialogueEmoteType.enumSize; i++){
                if( ((DialogueEmoteType)i).ToString().Equals(value) ){
                    return (DialogueEmoteType)i;
                }
            }
            Debug.LogError("No Emote Type found for string: " + value);
            return DialogueEmoteType.enumSize;
        }
    #endregion

    #region Dialogue State Management
        // Called in yarn scripts to remove speakers from StoryBeats, in the final node of branches; called as  <<BranchComplete [beatType] [nodeNameBase]>>
        // Must pass in a beatType (identical to the enum string value) and a nodeName (without the speakerID or # in the branch, just the base) IN THAT ORDER
        // NEVER call this on repeatable type dialogue -> it should never be removed from the dialogue pool bc it's repeatable
        public void BranchComplete(string[] info)
        {
            string beatTypeString = info[0];
            string nodeName = info[1];

            // Convert beat type string and find corresponding beat from node title
            StoryBeatType beatType = StoryManager.instance.GetBeatTypeFromString(beatTypeString);
            StoryBeat beat = StoryManager.instance.FindBeatFromNodeNameAndType(nodeName, beatType);

            if(beat == null){
                Debug.LogError("Failed to mark branch complete for null story beat");
                return;
            }

            Debug.Log("Removing " + beat.GetYarnHeadNode() + " from " + NPC.ActiveNPC.SpeakerData().SpeakerID());

            // Remove the speaker
            StoryManager.instance.RemoveSpeakerFromBeat(beat, NPC.ActiveNPC.SpeakerData().SpeakerID());
        }

        // Called by the Dialogue Runner to notify us that a node finished running
        public void NodeComplete(string nodeName)
        {
            // If we've already visited this node, just return (since nodes run lots of times in order to access current node in a branch)
            if(visitedNodes.Contains(nodeName)){
                return;
            }

            // Log that the node has been run
            visitedNodes.Add(nodeName);

            // Also alert the story manager in case there are any DialogueComplete nodes to add to the pool
            StoryManager.instance.ConversationEventOccurred(nodeName);
        }

        private void OnDialogueOpened()
        {
            // Stop allowing movement input
            InputManager.instance.ToggleDialogueOpenStatus(true);
            stopTime = true;

            // Disable UI elements
            InGameUIManager.instance.SetGameUIActive(false);
            AlertTextUI.instance.DisablePrimaryAlert();
            AlertTextUI.instance.DisableSecondaryAlert();
            InGameUIManager.instance.ToggleMiniMap(false);
        }

        // Called when the dialogue ends/closes
        public void OnDialogueEnd()
        {
            InputManager.instance.ToggleDialogueOpenStatus(false);
            InGameUIManager.instance.SetGameUIActive(true);

            stopTime = false;

            if(NPC.ActiveNPC){
                NPC.ActiveNPC.TalkedToNPC();
                if(NPC.ActiveNPC.SpeakerData().IsShopkeeper()){
                    // If you've only completed ONE run (or last run) and this is Stellan, don't open his shop this time
                    if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Stellan && (GameManager.instance.currentRunNumber == 2 || GameManager.instance.epilogueTriggered)){
                        // If the final run option, unlock the elevator
                        if(GameManager.instance.epilogueTriggered){
                            FindObjectOfType<SceneTransitionDoor>().GetComponent<Collider>().enabled = true;
                        }
                        return;
                    }
                    InGameUIManager.instance.OpenNPCShop(NPC.ActiveNPC.SpeakerData());
                }
                else if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.TimeLich){
                    FindObjectOfType<Lich>().canAttack = true;
                    InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(true, EnemyID.TimeLich);
                }
            }
            // If there's no active NPC and we're in the lich fight room AND the epilogue has NOT been triggered, kill Atlan
            else if( GameManager.instance.currentSceneName == GameManager.LICH_ARENA_STRING_NAME && !GameManager.instance.epilogueTriggered ){
                Player.instance.health.Damage( new DamageData(Player.instance.health.maxHitpoints, false), DamageSourceType.DefeatedTimeLichEndRunDeath );
            }
            // If there's no active NPC, deal with not lich-fight post-auto dialogue stuff
            else{
                int runNum = GameManager.instance.currentRunNumber;
                string scene = GameManager.instance.currentSceneName;

                // Both of the following if statements are kinda awful but I needed a way to make sure it was the right dialogue interaction
                // and that it wasn't just Stellan's comm auto dialogue after killing the beetle boi so

                // Force mini map open after grabbing the captain's log
                if(runNum == 1 && scene == GameManager.GAME_LEVEL1_STRING_NAME && PlayerInventory.hasCaptainsLog && !visitedNodes.Contains("PlayerStellanCommTriggered")){
                    InGameUIManager.instance.ToggleMiniMap(true);

                    // Journal alert
                    AlertTextUI.instance.EnableOpenJournalAlert();
                    StartCoroutine(AlertTextUI.instance.RemoveSecondaryAlertAfterSeconds());

                    // Expanded map alert
                    AlertTextUI.instance.EnableViewMapAlert();
                    StartCoroutine(AlertTextUI.instance.RemovePrimaryAlertAfterSeconds());
                }
                // Enable view stats alert after dialogue first time seeing stat reroll
                else if(runNum == 2 && scene == GameManager.GAME_LEVEL1_STRING_NAME && !PlayerInventory.instance.ItemSlotIsFull(InventoryItemSlot.Weapon)){
                    AlertTextUI.instance.EnableViewStatsAlert();
                    StartCoroutine(AlertTextUI.instance.RemovePrimaryAlertAfterSeconds());
                }
            }
        }

        // Called when the player clicks the interact button in range of an NPC with something to say
        // OR when we want to force dialogue when you walk into certain triggers at certain times (Time Lich, etc.)
        public void OnNPCInteracted()
        {
            OnDialogueOpened();

            if(NPC.ActiveNPC){
                dialogueRunner.StartDialogue(NPC.ActiveNPC.SpeakerData().GetYarnHeadNode());
            }
            else{
                dialogueRunner.StartDialogue( Player.instance.GetSpeakerData().GetYarnHeadNode() );
            }
        }
    #endregion

    public IEnumerator AutoRunDialogueAfterTime(float timeToWait = DEFAULT_AUTO_DIALOGUE_WAIT_TIME)
    {
        InputManager.instance.ToggleDialogueOpenStatus(true);   // Remove player control while waiting
        yield return new WaitForSeconds(timeToWait);

        // Play dialogue
        OnNPCInteracted();
    }
}

// Defines a comparer for StoryBeats (by priority)
public class CompareBeatsByPriority : IComparer<StoryBeat>
{
    public int Compare(StoryBeat beat1, StoryBeat beat2)
    {
        // Compare beat1 to beat2 by priority
        int priority1 = (int)beat1.GetPriority();
        int priority2 = (int)beat2.GetPriority();

        int difference = priority1 - priority2;

        // If they're the same, choose randomly between the two
        if( difference == 0 ){
            int num = Random.Range(0,2);
            difference = num == 0 ? 1 : -1;
        }

        return difference;
    }
}