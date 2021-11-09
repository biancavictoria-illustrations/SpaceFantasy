using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;


public enum DialogueTrigger
{
    // === Generic Triggers ===
    defaultDialogue,
    lowHealth,         // < 20%
    barterAttempt,
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

    private HashSet<string> visitedNodes = new HashSet<string>();   // Keeps track of what nodes the player has seen so that we don't see those again

    private int numGenericDialogueTriggers = 3;     // Number of distinct generic dialogue triggers

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
        // ... right?


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
                Change the structure so that it does something like:
                - right off the bat, if there's a MAX PRIORITY plotEvent, just play that
                - if not, keep looking -> gets the highest priority possible dialogue of each type
                - once you gather the options that are applicable, if there's a single Highest Priority Thing, play that
                - otherwise, choose randomly between everything on the same (highest) playing field
                - what if this means we never get generic/default dialogue??? could have an element of randomness also...
            */

            
            // Check for plot stuff and play the next relevant thing there if possible (then return)
            if( StoryManager.instance.activeStoryBeats.Count > 0 ){
                // Cycle the options and find the highest priority one
                // TODO: Try to find associated dialogue in descending priority order (this will only check the first one) (speaker data check should help maybe???)
                StoryBeat highestPriorityBeat = null;
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
                return highestPriorityBeat.GetYarnHeadNode();
            }
            // Maybe need to compare priority of ^^ to stuff below... (cuz there could be a lot of like "you killed a slime")


            // Check for run number related dialogue (values depend on the character you're talking to)
            int currentRunNumber = StoryManager.instance.currentRunNumber;
            foreach(int num in NPC.ActiveNPC.hasNumRunDialogueList){
                int difference = currentRunNumber - num;
                if( difference <= 3 && difference >= 0 ){
                    // return DialogueTrigger.numberOfRuns.ToString();
                }
                else if( difference > 3){
                    // Remove it from the list so we stop looking for it
                    NPC.ActiveNPC.hasNumRunDialogueList.Remove(num);
                }
            }


            // Check for item event triggers and play the next thing there if possible (then return)
            // TODO: Get a reference to the player's inventory and check if 
            // foreach item the player has
            // if( StoryManager.instance.itemDialogueTriggers.Contains(item) && thatItemTrigger.GetSpeakersWithComments().Contains(NPC.ActiveNPC.speakerData) )
            // return DialogueTrigger.hasItem.ToString();


            // === IF NO SPECIFIC EVENTS TRIGGERED, pick a random generic condition and do that ===

            // Get a random number from 0 - max generic dialogue triggers, then convert to the trigger enum value
            DialogueTrigger trigger = (DialogueTrigger)Random.Range(0,numGenericDialogueTriggers);

            // If bartering, check if success or fail
            PlayerStats playerStats = FindObjectsOfType<PlayerStats>()[0];
            if(trigger == DialogueTrigger.barterAttempt){
                if( playerStats.Charisma() >= 15 ){
                    return "barterSuccess";
                }
                else if( playerStats.Charisma() < 10 ){
                    return "barterFail";
                }
                else{
                    // If your CHA stat is 10-14
                    return "defaultDialogue";
                }
            }

            // If low HP, check if you're actually low HP
            if(trigger == DialogueTrigger.lowHealth){
                // TODO: Check if player IS low HP before including this option...
                // return "defaultDialogue";
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
            // Instead, play a default (NOT NEW) dialogue (TODO: add a new category for true default lame single line dialogue as opposed to default new progressing dialogue)


            return currentNode;
        });

        // Ideally have a SelectSpecific version for the node selection above? Idk specific dialogue is gonna have it's whole own system so we'll see
        // Might need specific versions for story events vs item events vs etc.
    }

    private string SelectPlotEventTopic()
    {
        return "";
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
        // Log that the node has been run.
        visitedNodes.Add(nodeName);
    }
}
