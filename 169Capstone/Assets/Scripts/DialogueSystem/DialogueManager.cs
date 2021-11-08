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

    private Dictionary<string, SpeakerData> speakers = new Dictionary<string, SpeakerData>();

    public Image characterPortrait;
    public TMP_Text speakerName;

    public DialogueRunner dialogueRunner;

    // Keeps track of what nodes the player has seen so that we don't see those again
    private HashSet<string> visitedNodes = new HashSet<string>();

    // Number of distinct generic dialogue triggers
    private int numGenericDialogueTriggers = 3;

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
            PlayerStats playerStats = FindObjectsOfType<PlayerStats>()[0];

            // TODO: Check for plot stuff and play the next relevant thing there if possible (then return)
            // return DialogueTrigger.plotEvents.ToString();
            
            // TODO: Check for # runs triggers and play the next thing there if possible (then return)
            // return DialogueTrigger.numberOfRuns.ToString();

            // TODO: Check for item event triggers and play the next thing there if possible (then return)
            // return DialogueTrigger.hasItem.ToString();

            // === IF NO SPECIFIC EVENTS TRIGGERED, pick a random generic condition and do that ===

            // Get a random number from 0 - max generic dialogue triggers, then convert to the trigger enum value
            DialogueTrigger trigger = (DialogueTrigger)Random.Range(0,numGenericDialogueTriggers);
            // If bartering, check if success or fail
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
                // Check if player is NOT actually low HP, and if that's the case set to default instead
                // return "defaultDialogue";
                // or if low HP should be higher priority, could check for it in a priority order instead of the random generation?
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
    }

    public void AddSpeaker(SpeakerData data)
    {
        if(speakers.ContainsKey(data.speakerName)){
            Debug.Log("Attempting to add " + data.speakerName + " to the speaker database, but it already exists!");
            return;
        }
        speakers.Add(data.speakerName, data);
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
        Debug.Log("Could not set the speaker data for " + speaker);
    }

    // Called by the Dialogue Runner to notify us that a node finished running
    public void NodeComplete(string nodeName)
    {
        // Log that the node has been run.
        visitedNodes.Add(nodeName);
    }
}
