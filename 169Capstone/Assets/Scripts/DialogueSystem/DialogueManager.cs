using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;


// Types of dialogue -> the things the NPC responds to
public enum DialogueTrigger
{
    // === Generic Triggers ===
    defaultDialogue,
    lowHealth,         // < 20%
    barterAttempt,
    repeatableGeneric,
    // === Specific Barter Conditions ===
    barterSuccess,     // >=15 CHA
    barterFail,        // < 10 CHA
    // === High Priority Triggers ===
    numRuns,
    item,
    plotEvents,        // Killed creature, killed by creature, finished conversation (handled by the StoryManager)
    // === For Looping Purposes ===
    enumSize
}

public class DialogueManager : MonoBehaviour
{   
    public static DialogueManager instance;

    public GameObject dialogueUICanvas;

    private Dictionary<string, SpeakerData> speakers = new Dictionary<string, SpeakerData>();

    public Image characterPortrait; 
    public TMP_Text speakerName;

    public DialogueRunner dialogueRunner;

    private HashSet<string> visitedNodes = new HashSet<string>();       // Keeps track of what nodes the player has seen so that we don't see those again
    private HashSet<string> visitedBranches = new HashSet<string>();    // Keeps track of what ENTIRE BRANCHES have been used up so that we don't even have to look at those again
        // Stores the string of the HEAD NODE for that branch
        // Neither visited hashset stores repeatableGeneric type nodes

    private int numGenericDialogueTriggers = 4;     // Number of distinct generic dialogue triggers

    void Awake()
    {
        // Make this a singleton so that it can be accessed from anywhere and there's only one
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);     // ... right? (VERIFY THIS)


        // Add SetSpeakerInfo to yarn so that we can set character portraits and names
        dialogueRunner.AddCommandHandler("SetSpeaker", SetSpeakerInfo);

        // Add Visited to yarn so that we can check if a node has been visited yet
        dialogueRunner.AddFunction("Visited", 1, delegate (Yarn.Value[] parameters){
            var nodeName = parameters[0];
            return visitedNodes.Contains(nodeName.AsString);
        });


        // Add SelectNextTrigger so that we can find the next conditional category of dialogue to play
        dialogueRunner.AddFunction("SelectNextTrigger", 0, delegate (Yarn.Value[] parameters){
            
            /*
                - if not, keep looking -> gets the highest priority possible dialogue of each type
                - once you gather the options that are applicable, if there's a single Highest Priority Thing, play that
                - otherwise, choose randomly between everything on the same (highest) playing field
                

                Current Problems/TODO:
                ======================

                - Need a BranchVisited function so that you can not only mark NODES as visited but entire BRANCHES starting from the YarnHeadNodes
                defined in PlotBeats and stuff
                    -> Once Yarn reaches the end of a branch, it marks the entire branch as visited
                    -> Use NodeVisited stuff as a reference
                    -> Mark the branch as visited, and then be able to check if a branch is visited HERE in Unity so that we can say
                    if(BranchVisited){ pick a new topic/interaction/branch }
                    -> Should entirely visited/expended branches just be removed from the available dialogue pool IN GENERAL? That's stored in
                    the StoryManager so that could be doable...
                    -> Hence also the need for a Generic Default Dialogue category that isn't just "can be played whenever" but a REPEATABLE pool
                    (called Repeatable presumably, and the default if there are NO other options, or as occasional default/generic options so
                    it's less jarring/obvious; single generic repeatable line that the NPC can say)
                
                - if certain plot beats (like killing slimes) always show up, we're never going to get any other options for the highest priorty (bc it'll keep
                picking the first one seen which will always be the same I think???)
                    -> like they're not ranked INDIVIDUALLY, they're priority CATEGORIES
                    -> we need a way to randomly select one of the many options on the same priority level
                    -> things need to be built into repeatable functions that you can 
                
                - so a lot of this seems to indicate we need this to be broken down into individual functions returning these values so that it can be more
                modular and we can redo things in loops as necessary, basically
                    -> like "loop through this thing and do this until you find a satisfactory* option, then move on to the next category and do the same"
                    -> *branch NOT entirely completed yet for the currently active NPC (has new dialogue), all plot beat requirements met and triggered, etc.
            */

            StoryBeat highestPriorityBeat = null;
            
            // Check for plot stuff and play the next relevant thing there if possible (then return)
            if( StoryManager.instance.activeStoryBeats.Count > 0 ){
                // Cycle the options and find the highest priority one
                foreach(StoryBeat beat in StoryManager.instance.activeStoryBeats){
                    // Set default value (first option)
                    if( highestPriorityBeat == null ){
                        highestPriorityBeat = beat;
                    }
                    // If this NPC has something to say AND the priority is higher, set it to the interaction we'll choose
                    else if( beat.GetPriority() > highestPriorityBeat.GetPriority() && beat.GetSpeakersWithComments().Contains(NPC.ActiveNPC.speakerData) ){
                        highestPriorityBeat = beat;
                    }
                }
            }

            // If the current highest priority beat is MAX PRIORITY (Big Plot Events), just go ahead and return that now
            if( highestPriorityBeat != null && highestPriorityBeat.GetPriority() == DialoguePriority.maxPriority ){
                return highestPriorityBeat.GetYarnHeadNode();
            }

            // If the current highest priority beat is P1 or lower, check for current run number dialogue instead
            if( highestPriorityBeat != null && highestPriorityBeat.GetPriority() <= DialoguePriority.p1 ){
                // Values depend on the character you're talking to
                int currentRunNumber = StoryManager.instance.currentRunNumber;
                foreach(int num in NPC.ActiveNPC.hasNumRunDialogueList){
                    int difference = currentRunNumber - num;
                    if( difference <= 3 && difference >= 0 ){
                        return DialogueTrigger.numRuns.ToString();
                    }
                    else if( difference > 3){
                        // Remove it from the list so we stop looking for it
                        NPC.ActiveNPC.hasNumRunDialogueList.Remove(num);    // (VERIFY) Can you do this in a foreach loop or does that mess it up?
                    }
                }
            }

            // Check for item event triggers and if any are met, find the highest priority item trigger
            // TODO: Get a reference to the player's inventory (remember to check if it's empty first presumably? unless foreach handles that well) and make this not pseudocode
            // foreach( Item item in playerInventory ){
            //     // TODO: Get that item's item trigger, if it has one (the below thing doesn't work for that)
            //     if(StoryManager.instance.itemDialogueTriggers.Contains(itemTrigger) && itemTrigger.GetSpeakersWithComments().Contains(NPC.ActiveNPC.speakerData)){
            //         if( highestPriorityBeat == null ){
            //             highestPriorityBeat = itemTrigger;
            //         }
            //         // Compare priority of the item trigger to the plot event trigger
            //         if( itemTrigger.GetPriority() > highestPriorityBeat.GetPriority() ){
            //             highestPriorityBeat = itemTrigger;
            //         }
            //     }
            // }

            // If at this point the highest priority beat exists and is > P1, play that
            if( highestPriorityBeat != null && highestPriorityBeat.GetPriority() > DialoguePriority.p1 ){
                return highestPriorityBeat.GetYarnHeadNode();
            }

            // === Now include more generic/less high priority dialogue checks ===

            PlayerStats playerStats = FindObjectsOfType<PlayerStats>()[0];

            // Default trigger value
            DialogueTrigger trigger = DialogueTrigger.defaultDialogue;

            // Check if the player is low health
            bool isLowHP = false;
            // TODO: Check if the player is <= 20% health (like currentHP / maxHP <= 0.2)
            if( playerStats.getMaxHitPoints() == 0 ){
                isLowHP = true;
            }

            // Check if you can get successful or failed barter attempt dialogue
            DialogueTrigger canAttemptBarter = DialogueTrigger.enumSize;
            if( playerStats.Charisma() >= 15 ){
                canAttemptBarter = DialogueTrigger.barterSuccess;
            }
            else if( playerStats.Charisma() < 10 ){
                canAttemptBarter = DialogueTrigger.barterFail;
            }

            // If both are options
            if( isLowHP && canAttemptBarter != DialogueTrigger.enumSize ){
                // Includes low HP, barter attempt, and default dialogue as options
                // DOES NOT INCLUDE generic repeatable, which only plays if there's nothing new or good to play
                trigger = (DialogueTrigger)Random.Range(0,numGenericDialogueTriggers-1);
            }
            // If only one of the two is an option
            else if( isLowHP || canAttemptBarter != DialogueTrigger.enumSize ){
                // Get a random number 0 or 1
                // If 1, set trigger to lowHP or the barter attempt value, depending on which was true; (0 stays default)
                int randomValue = Random.Range(0,2);
                if( randomValue == 1 ){
                    trigger = isLowHP ? DialogueTrigger.lowHealth : canAttemptBarter;
                }
            }

            // If we had a plot beat from before, randomly pick between that or this random interaction
            if( highestPriorityBeat != null ){
                int randomValue = Random.Range(0,2);
                if(randomValue == 1){
                    return highestPriorityBeat.GetYarnHeadNode();
                }
            }

            return trigger.ToString();        
        });


        // Add a function to tell yarn which node in that conditional branch to play
        dialogueRunner.AddFunction("SelectGenericNode", 1, delegate (Yarn.Value[] parameters){
            // Takes in a trigger; search the active NPC's dictionary for that key, the value is an int of where we're at
            DialogueTrigger trigger = DialogueTrigger.defaultDialogue;
            for(int i = 0; i < (int)DialogueTrigger.enumSize; ++i){
                if(( (DialogueTrigger)i ).ToString().Equals(parameters[0])){
                    trigger = (DialogueTrigger)i;
                }
            }
            int currentNode = NPC.ActiveNPC.genericDialogueTriggers[trigger];   // Set the current position in the branch
            NPC.ActiveNPC.genericDialogueTriggers[trigger] = currentNode + 1;   // Increment the value in the dictionary

            // TODO: If this condition is now out of interactions, remove it from the pool (NPC.ActiveNPC.RemoveDialogueTrigger(trigger))
            // Update: based on recent changes (aka the entire StoryManager + StoryBeat system) that might not be the way to do it in general
            // but yes we need some kind of BranchVisited function and stuff

            return currentNode;
        });


        // TODO: Selecting a specific node for NON-GENERIC dialogue (aka PlotBeats :)  -> items, killed, & conversation events)
        // Probably maybe needs its own function added to yarn for this...


        // TODO: Selecting a specific node for runNum interactions, based on current run num & when this speaker comments on that :)
    }

    public void AddSpeaker(SpeakerData data)
    {
        if(speakers.ContainsKey(data.SpeakerName())){
            Debug.LogError("Attempting to add " + data.SpeakerName() + " to the speaker database, but it already exists!");
            return;
        }
        speakers.Add(data.SpeakerName(), data);
    }

    private void SetSpeakerInfo(string[] info)
    {
        // Set speaker name
        string speaker = info[0];

        // Set portrait emotion
        string emotion = "";
        if(info.Length > 1){
            emotion = info[1];
        }
        else{
            emotion = SpeakerData.EMOTION_NEUTRAL;
        }
        
        if(speakers.TryGetValue(speaker, out SpeakerData data)){
            // TODO: Uncomment once we have sprites :)
            // characterPortrait.sprite = data.GetEmotionPortrait(emotion);
            speakerName.text = speaker;
            return;
        }
        Debug.LogError("Could not set the speaker data for " + speaker);
    }

    // Called by the Dialogue Runner to notify us that a node finished running
    public void NodeComplete(string nodeName)
    {
        // Log that the node has been run
        visitedNodes.Add(nodeName);
    }
}
