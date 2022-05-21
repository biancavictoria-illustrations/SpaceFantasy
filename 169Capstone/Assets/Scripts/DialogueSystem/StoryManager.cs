using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StoryManager : MonoBehaviour
{
    // Struct for storing data about story beats that changes at runtime (current state stuff)
    [System.Serializable] public struct BeatStatus{
        public string storyBeatHeadNode;
        public int storyBeatType;

        public bool beatIsActive;          // If the player did this thing on their latest run (so characters can/should respond to it)
        public int numberOfCompletions;    // The number of times the player has done this thing

        public int[] speakersWithComments; // Things are removed once they no longer have new things to say on a topic
        
        // Constructor
        public BeatStatus(string headNode, bool active, int num, List<SpeakerID> speakers, int beatType){
            storyBeatHeadNode = headNode;
            storyBeatType = beatType;

            speakersWithComments = new int[speakers.Count];
            // Add all the speakerIDs
            for(int i = 0; i < speakers.Count; ++i){
                speakersWithComments[i] = (int)speakers[i];
            }

            this.beatIsActive = active;
            this.numberOfCompletions = num;
        }

        // Constructor w/ Array instead of List
        public BeatStatus(string headNode, bool active, int num, int[] speakers, int beatType){
            storyBeatHeadNode = headNode;
            storyBeatType = beatType;

            speakersWithComments = new int[speakers.Length];
            // Add all the speakerIDs
            for(int i = 0; i < speakers.Length; ++i){
                speakersWithComments[i] = speakers[i];
            }

            this.beatIsActive = active;
            this.numberOfCompletions = num;
        }

        public bool SpeakerHasComments(int speakerIDNum)
        {
            foreach(int num in speakersWithComments){
                if(num == speakerIDNum){
                    return true;
                }
            }
            return false;
        }

        public void RemoveSpeaker(int speakerIDNum)
        {
            int index = -1;
            for(int i = 0; i < speakersWithComments.Length; i++){
                if(speakersWithComments[i] == speakerIDNum){
                    index = i;
                }
            }
            if(index == -1){
                Debug.LogWarning("Speaker not found in beat!");
                return;
            }
            speakersWithComments[index] = -1;
        }
    }

    public static StoryManager instance;

    [HideInInspector] public bool talkedToBryn, talkedToStellan, talkedToRhian, talkedToDoctor, talkedToSorrel, talkedToLich = false;

    [HideInInspector] public List<int> brynNumRunDialogueList = new List<int>();
    [HideInInspector] public bool brynListInitialized;

    [HideInInspector] public List<int> stellanNumRunDialogueList = new List<int>();
    [HideInInspector] public bool stellanListInitialized;

    [HideInInspector] public List<int> timeLichNumRunDialogueList = new List<int>();
    [HideInInspector] public bool lichListInitialized;

    // [HideInInspector] public List<int> doctorNumRunDialogueList = new List<int>();
    // [HideInInspector] public bool doctorListInitialized;

    // [HideInInspector] public List<int> rhianNumRunDialogueList = new List<int>();
    // [HideInInspector] public bool rhianListInitialized;

    public Dictionary<StoryBeat,BeatStatus> storyBeatDatabase = new Dictionary<StoryBeat,BeatStatus>();     // All story beats of type Conversation or Killed
    public HashSet<StoryBeat> activeStoryBeats = new HashSet<StoryBeat>();    // For the DialogueManager to see just the active beats

    // TODO: Update their beat status values??? -> UpdateBeatStatus() can do that for them too, just need to call it somewhere
    // But if calling them to set them active somewhere, like in the DialogueManager, should also set them inactive after (end of a run presumably)
    public Dictionary<StoryBeatItem,BeatStatus> itemStoryBeats = new Dictionary<StoryBeatItem,BeatStatus>();  // All item dialogue triggers
    public Dictionary<StoryBeat,BeatStatus> genericStoryBeats = new Dictionary<StoryBeat,BeatStatus>();     // For attemptBarter, lowHP, default, and repeatable

    void Awake()
    {
        // Make this a singleton so that it can be accessed from anywhere and there's only one
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        if(storyBeatDatabase.Count == 0){
            LoadAllStoryBeats();
        }
    }

    public void InitializeStoryManagerOnNewGame()
    {
        brynListInitialized = false;
        stellanListInitialized = false;
        lichListInitialized = false;
        // doctorListInitialized = false;
        // rhianListInitialized = false;

        activeStoryBeats.Clear();

        // Set all story beat statuses to default values
        storyBeatDatabase.Clear();
        itemStoryBeats.Clear();
        genericStoryBeats.Clear();
        LoadAllStoryBeats();
    }

    public void LoadSavedStoryBeatStatuses(BeatStatus[] storyBeatDatabaseStatuses, BeatStatus[] itemStoryBeatStatuses, BeatStatus[] genericStoryBeatStatuses, string[] activeStoryBeatHeadNodes)
    {
        for(int i = 0; i < storyBeatDatabaseStatuses.Length; i++){
            BeatStatus status = storyBeatDatabaseStatuses[i];
            StoryBeat beat = FindBeatFromNodeNameAndType(status.storyBeatHeadNode, (StoryBeatType)status.storyBeatType);
            storyBeatDatabase[beat] = status;
        }

        for(int i = 0; i < itemStoryBeatStatuses.Length; i++){
            BeatStatus status = itemStoryBeatStatuses[i];
            StoryBeatItem beat = (StoryBeatItem)FindBeatFromNodeNameAndType(status.storyBeatHeadNode, (StoryBeatType)status.storyBeatType);
            itemStoryBeats[beat] = status;
        }

        for(int i = 0; i < genericStoryBeatStatuses.Length; i++){
            BeatStatus status = genericStoryBeatStatuses[i];
            StoryBeat beat = FindBeatFromNodeNameAndType(status.storyBeatHeadNode, (StoryBeatType)status.storyBeatType);
            genericStoryBeats[beat] = status;
        }

        activeStoryBeats.Clear();
        for(int i = 0; i < activeStoryBeatHeadNodes.Length; i++){
            activeStoryBeats.Add( FindBeatFromNodeNameOnly(activeStoryBeatHeadNodes[i]) );
        }
    }

    private void LoadAllStoryBeats()
    {
        // Load in the StoryBeats (only of type Killed and Conversation) from the StoryBeats folder in Resources
        Object[] storyBeatList = Resources.LoadAll("SpecificStoryBeats", typeof(StoryBeat));
        foreach(Object s in storyBeatList){
            StoryBeat beat = (StoryBeat)s;
            
            if(storyBeatDatabase.ContainsKey(beat)){
                continue;
            }

            beat.CreatePrereqDatabase();
            beat.SetValues();
            // Add the storybeat to the dictionary
            storyBeatDatabase.Add( beat, new BeatStatus( beat.GetYarnHeadNode(), false, 0, beat.GetSpeakersWithComments(), (int)beat.GetBeatType() ) );
        }

        // Load in the ItemDialogueTriggers from the ItemTriggers folder in Resources
        Object[] itemDialogueList = Resources.LoadAll("ItemStoryBeats", typeof(StoryBeatItem));
        foreach(Object i in itemDialogueList){
            StoryBeatItem beat = (StoryBeatItem)i;

            if(itemStoryBeats.ContainsKey(beat)){
                continue;
            }

            beat.CreatePrereqDatabase();
            beat.SetValues();
            itemStoryBeats.Add(beat,new BeatStatus( beat.GetYarnHeadNode(), false, 0, beat.GetSpeakersWithComments(), (int)beat.GetBeatType() ));
        }

        // Load in the generic story beats from the GenericStoryBeats folders in Resources
        Object[] genericDialogueList = Resources.LoadAll("GenericStoryBeats", typeof (StoryBeat));
        foreach(Object g in genericDialogueList){
            StoryBeat beat = (StoryBeat)g;

            if(genericStoryBeats.ContainsKey(beat)){
                continue;
            }

            beat.CreatePrereqDatabase();
            beat.SetValues();
            genericStoryBeats.Add( beat, new BeatStatus( beat.GetYarnHeadNode(), false, 0, beat.GetSpeakersWithComments(), (int)beat.GetBeatType() ) );
        }
    }

    // Called by Game Manager when you end a run
    public void OnRunEndUpdateStory()
    {
        // TODO: Increment story beat values (i think that's already happening in the CheckForNewStoryBeats function???)

        // When you end a run, reset all talked to bools
        talkedToBryn = false;
        talkedToStellan = false;
        talkedToRhian = false;
        talkedToDoctor = false;
        talkedToSorrel = false;
        talkedToLich = false;
    }

    // Returns true if the speakerID is in that beat's speaker list at this time
    public bool SpeakerIsInSpeakerList(StoryBeat beat, SpeakerID speakerID)
    {
        StoryBeatType beatType = beat.GetBeatType();
        bool flag = true;

        // If it's a Killed Event or Conversation Event
        if((beatType == StoryBeatType.KilledBy || beatType == StoryBeatType.EnemyKilled || beatType == StoryBeatType.DialogueCompleted) && (!BeatIsInDatabase(beat) || !storyBeatDatabase[beat].SpeakerHasComments((int)speakerID))){
            flag = false;
        }
        // If it's an item
        else if(beatType == StoryBeatType.Item){
            StoryBeatItem item = (StoryBeatItem)beat;
            if( !BeatIsInDatabase(beat) || !itemStoryBeats[item].SpeakerHasComments((int)speakerID) ){
                flag = false;
            }
        }
        // If it's a generic storybeat
        else if(((int)beatType <= 5) && (!BeatIsInDatabase(beat) || !genericStoryBeats[beat].SpeakerHasComments((int)speakerID))){
            flag = false;
        }
        if(flag == false){
            Debug.LogWarning("Failed to access " + speakerID + " in beat " + beat.GetYarnHeadNode() + "'s speaker list!");
        }
        return flag;
    }

    private bool BeatIsInDatabase(StoryBeat beat)
    {
        StoryBeatType beatType = beat.GetBeatType();
        bool flag = true;
        // If it's a Killed Event or Conversation Event
        if((beatType == StoryBeatType.KilledBy || beatType == StoryBeatType.EnemyKilled || beatType == StoryBeatType.DialogueCompleted) && (!storyBeatDatabase.ContainsKey(beat))){
            flag = false;
        }
        // If it's an item
        else if(beatType == StoryBeatType.Item){
            StoryBeatItem item = (StoryBeatItem)beat;
            if( !itemStoryBeats.ContainsKey(item) ){
                flag = false;
            }
        }
        // If it's a generic storybeat
        else if((int)beatType <= 5 && !genericStoryBeats.ContainsKey(beat)){
            flag = false;
        }
        if( flag == false ){
            Debug.LogError("Tried to access beat " + beat.GetYarnHeadNode() + " of type " + beat.GetBeatType() + ", but the beat is not in the database!");
        }
        return flag;
    }

    // Removes a speaker from a given beat's list of speakers who have something to say about that beat
    // Called once an entire branch starting from that beat's head node is completed -> that way we no longer consider that in that speaker's pool of things to say
    public void RemoveSpeakerFromBeat(StoryBeat beat, SpeakerID speakerID)
    {
        StoryBeatType beatType = beat.GetBeatType();
        if( SpeakerIsInSpeakerList(beat, speakerID) ){
            // If Killed or Conversation Event
            if( beatType == StoryBeatType.EnemyKilled || beatType == StoryBeatType.KilledBy || beatType == StoryBeatType.DialogueCompleted ){
                // VERIFY that this works; might not bc of struct stuff, in which case would have to make a new BeatStatus with a new hash set that doesn't have that speakerID
                storyBeatDatabase[beat].RemoveSpeaker((int)speakerID);
            }
            // If item
            else if( beatType == StoryBeatType.Item ){
                StoryBeatItem item = (StoryBeatItem)beat;
                itemStoryBeats[item].RemoveSpeaker((int)speakerID);
            }
            // If generic
            else if( (int)beatType <= 5 ){
                genericStoryBeats[beat].RemoveSpeaker((int)speakerID);
            }
        }
    }

    // Find the StoryBeat corresponding to a given node name string, given a beat type
    public StoryBeat FindBeatFromNodeNameAndType(string nodeName, StoryBeatType beatType)
    {
        // If Killed or Conversation Event search storyBeatDatabase
        if( beatType == StoryBeatType.EnemyKilled || beatType == StoryBeatType.KilledBy || beatType == StoryBeatType.DialogueCompleted ){
            foreach(StoryBeat beat in storyBeatDatabase.Keys){
                if( beat.GetYarnHeadNode().Equals(nodeName) ){
                    return beat;
                }
            }
        }
        // If item search itemDialogueTriggers
        else if( beatType == StoryBeatType.Item ){
            foreach(StoryBeatItem beat in itemStoryBeats.Keys){
                if( beat.GetYarnHeadNode().Equals(nodeName) ){
                    return beat;
                }
            }
        }
        // If generic search genericStoryBeats
        else if( (int)beatType <= 5 ){
            foreach(StoryBeat beat in genericStoryBeats.Keys){
                if( beat.GetYarnHeadNode().Equals(nodeName) ){
                    return beat;
                }
            }
        }
        else{
            Debug.LogWarning("No beats found for beat type: " + beatType);
            return null;
        }
        // Debug.LogWarning("No beats found for node name " + nodeName + " with beat type " + beatType);
        return null;
    }

    // Less efficient way of tracking down a story beat, as it has to check ALL the databases. Only use if necessary (like when loading save data)
    public StoryBeat FindBeatFromNodeNameOnly(string nodeName)
    {
        // Check all 3 story beat databases for a matching node name; return the first match found or null if none match
        foreach(StoryBeat beat in storyBeatDatabase.Keys){
            if( beat.GetYarnHeadNode().Equals(nodeName) ){
                return beat;
            }
        }
        foreach(StoryBeatItem beat in itemStoryBeats.Keys){
            if( beat.GetYarnHeadNode().Equals(nodeName) ){
                return beat;
            }
        }
        foreach(StoryBeat beat in genericStoryBeats.Keys){
            if( beat.GetYarnHeadNode().Equals(nodeName) ){
                return beat;
            }
        }
        Debug.LogWarning("No beats found for node name " + nodeName);
        return null;
    }

    // Takes in a string that should match a StoryBeatType enum string value perfectly; convert to the enum value
    public StoryBeatType GetBeatTypeFromString(string beatTypeString)
    {
        StoryBeatType beatType = StoryBeatType.enumSize;
        for(int i = 0; i < (int)StoryBeatType.enumSize; ++i){
            if( ((StoryBeatType)i).ToString() == beatTypeString ){
                beatType = (StoryBeatType)i;
            }
        }
        if( beatType == StoryBeatType.enumSize ){
            Debug.LogError("No story beat type found for string: " + beatTypeString + ". Beat type set to " + beatType + ".");
        }
        return beatType;
    }

    // Update beat status to reflect the given active/inactive bool value + increment by the num provided (either 0 or 1)
    private void UpdateBeatStatus(StoryBeat beat, bool setActive, int incrementCompletionNum)
    {
        StoryBeatType beatType = beat.GetBeatType();
        if( BeatIsInDatabase(beat) ){
            // If Killed or Conversation Event
            if( beatType == StoryBeatType.EnemyKilled || beatType == StoryBeatType.KilledBy || beatType == StoryBeatType.DialogueCompleted ){
                storyBeatDatabase[beat] = new BeatStatus( beat.GetYarnHeadNode(), setActive, storyBeatDatabase[beat].numberOfCompletions + incrementCompletionNum, storyBeatDatabase[beat].speakersWithComments, (int)beatType );
            }
            // If item
            else if( beatType == StoryBeatType.Item ){
                StoryBeatItem item = (StoryBeatItem)beat;
                itemStoryBeats[item] = new BeatStatus( item.GetYarnHeadNode(), setActive, storyBeatDatabase[beat].numberOfCompletions + incrementCompletionNum, storyBeatDatabase[beat].speakersWithComments, (int)beatType );
            }
            // If generic
            else if( (int)beatType <= 5 ){
                genericStoryBeats[beat] = new BeatStatus( beat.GetYarnHeadNode(), setActive, storyBeatDatabase[beat].numberOfCompletions + incrementCompletionNum, storyBeatDatabase[beat].speakersWithComments, (int)beatType );
            }
        }
    }

    // When you achieve this story beat on a run, increment the # completions and set achieved to true
    private void AchievedStoryBeat(StoryBeat beat, string talkedToNPCName = "")
    {
        if( beat.GetBeatType() != StoryBeatType.EnemyKilled && beat.GetBeatType() != StoryBeatType.KilledBy && beat.GetBeatType() != StoryBeatType.DialogueCompleted ){
            Debug.LogError("Tried to call AchievedStoryBeat on wrong beat type: " + beat.GetBeatType() + " " + beat.GetYarnHeadNode() + "!");
            return;
        }

        // If there are prereqs, check them
        if(beat.GetPrereqStoryBeats().Count > 0){
            Dictionary<StoryBeat,int> prereqs = beat.GetPrereqStoryBeats();
            foreach(StoryBeat p in prereqs.Keys){
                if( storyBeatDatabase.ContainsKey(p) ){
                    // If one of the prereqs has not been met, don't mark this story beat as active
                    if( storyBeatDatabase[p].numberOfCompletions < prereqs[p] ){
                        Debug.Log("Prereqs NOT met in order to add Story Beat " + beat.GetYarnHeadNode());
                        return;
                    }
                }
                else if( (int)p.GetBeatType() <= 5 ){   // If generic, check for the specific node
                    if( DialogueManager.instance.visitedNodes.Contains( talkedToNPCName + p.GetYarnHeadNode() + (prereqs[p] - 1) ) ){
                        Debug.Log("Prereqs NOT met in order to add Story Beat. visitedNodes does not contain " + talkedToNPCName + p.GetYarnHeadNode() + (prereqs[p] - 1));
                        return;
                    }
                }
                else{
                    Debug.LogWarning("Story beat with type " + p.GetBeatType() + " not found while checking prereqs");
                }
            }
        }

        // If all potential prereqs are met, update the story beat status to be active and increment completion
        UpdateBeatStatus(beat, true, 1);
    }

    // At the end of every run, check if any new story beats have been activated; if so, add them to the list for the DialogueManager to access
    public void CheckForNewStoryBeats()
    {
        // Reset active story beats
        activeStoryBeats.Clear();

        // Check if any story beats have been set active, and if so add them to the list for the dialogue runner and set new story available to true
        foreach( StoryBeat beat in storyBeatDatabase.Keys ){
            if( storyBeatDatabase[beat].beatIsActive ){
                activeStoryBeats.Add(beat);
            }
        }

        // Now that the latest achieved have been queued, reset everything that doesn't carry over to not active
        // This goes here because once they're in the activeStoryBeats list they're queued up to be checked on the next run
        // Going into the next run, we want a fresh start to start logging new values for THAT run
        foreach( StoryBeat beat in activeStoryBeats ){
            if( !beat.CarriesOver() ){
                UpdateBeatStatus(beat, false, 0);
            }
        }
    }

    // Called when the event is invoked either by killing a creature OR being killed by a creature
    public void KilledEventOccurred(EnemyID enemy, StoryBeatType beatType)
    {
        // If this event's beatType is NOT creatureKilled, error
        if( beatType != StoryBeatType.EnemyKilled ){
            Debug.LogError("Creature Killed event occurred for wrong StoryBeatType: " + beatType + " " + enemy + "!");
            return;
        }

        StoryBeat beat = FindBeatFromNodeNameAndType(beatType.ToString() + enemy, beatType);
        
        if(beat != null)
            AchievedStoryBeat(beat);
    }

    public void KilledEventOccurred(DamageSourceType damageSource, StoryBeatType beatType)
    {
        // If this event's beatType is NOT killedBy, error
        if( beatType != StoryBeatType.KilledBy ){
            Debug.LogError("Killed By event occurred for wrong StoryBeatType: " + beatType + " " + damageSource + "!");
            return;
        }

        StoryBeat beat = FindBeatFromNodeNameAndType(beatType.ToString() + damageSource, beatType);
        
        if(beat != null)
            AchievedStoryBeat(beat);
    }

    public void ConversationEventOccurred(SpeakerID npc, string dialogueHeadNode)
    {
        // Log that a dialogue completed beat occured with this NPC and this head node, if one is found
        string nodeName = StoryBeatType.DialogueCompleted.ToString() + npc.ToString() + dialogueHeadNode;
        StoryBeat beat = FindBeatFromNodeNameAndType(nodeName, StoryBeatType.DialogueCompleted);
        
        if(beat != null){
            AchievedStoryBeat(beat, npc.ToString());
        }
        else{
            Debug.LogWarning("No beat found/updated for " + nodeName);
        }
    }
}
