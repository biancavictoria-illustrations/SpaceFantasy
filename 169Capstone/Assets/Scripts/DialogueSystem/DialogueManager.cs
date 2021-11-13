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

    private HashSet<string> visitedNodes = new HashSet<string>();       // Keeps track of what nodes the player has seen so that we don't see those again

    [SerializeField] private int numRunsThreshold = 3;   // Threshold for # runs beyond the exact num run that numRun dialogue can trigger

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

        // Add commands to Yarn so that we can send Unity info from there
        dialogueRunner.AddCommandHandler("SetSpeaker", SetSpeakerUI);
        dialogueRunner.AddCommandHandler("BranchComplete", BranchComplete);

        // Add Visited to yarn so that we can check if a node has been visited yet
        dialogueRunner.AddFunction("Visited", 1, delegate (Yarn.Value[] parameters){
            var nodeName = parameters[0];
            return visitedNodes.Contains(nodeName.AsString);
        });


        // Add SelectNextTrigger so that we can find the next conditional category of dialogue to play
        dialogueRunner.AddFunction("SelectNextNode", 0, delegate (Yarn.Value[] parameters){
            // Sorted database of all beats available at the moment (all for which conditions are met)
            SortedSet<StoryBeat> sortedStoryBeats = new SortedSet<StoryBeat>(new CompareBeatsByPriority());
            
            // Check for Killed Events and Conversation Events
            if( StoryManager.instance.activeStoryBeats.Count > 0 ){
                // Cycle the options and add ones for which the active NPC has something to say
                foreach(StoryBeat beat in StoryManager.instance.activeStoryBeats){
                    if( StoryManager.instance.SpeakerIsInSpeakerList(beat, NPC.ActiveNPC.speakerData.SpeakerID()) ){
                        sortedStoryBeats.Add(beat);
                    }
                }
            }

            // // Check for item event triggers and if any are met, find the highest priority item trigger
            // // TODO: Get a reference to the player's inventory (remember to check if it's empty first presumably? unless foreach handles that well) and make this not pseudocode
            // foreach( string itemName in playerInventory ){
            //     // TODO: Check if this item has a story beat associated with it (don't break everything if not, but if so save it to item)
            //     StoryBeat item = StoryManager.instance.FindBeatFromNodeName(itemName, StoryBeatType.item);
            //     // If the active NPC has nothing to say about this beat, skip it
            //     if( !StoryManager.instance.SpeakerIsInSpeakerList(item, NPC.ActiveNPC.speakerData.SpeakerID()) ){
            //         continue;
            //     }
            //     // Otherwise, add it to the pool of dialogue options
            //     sortedStoryBeats.Add(item);
            // }

            PlayerStats playerStats = FindObjectsOfType<PlayerStats>()[0];
            
            // If we see a numRun dialogue interaction, we need to be able to remove that option from the pool later
            int currentNumRunRemoveValue = 0;   

            // Loop through genericDialogueList and check if the conditions are met for each trigger type, then add if necessary
            foreach( StoryBeat beat in StoryManager.instance.genericStoryBeats.Keys ){
                // Confirm that the active NPC has something to say about this beat; if not, skip it and move on to the next option
                if( !StoryManager.instance.SpeakerIsInSpeakerList(beat, NPC.ActiveNPC.speakerData.SpeakerID()) ){
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
                else if( beatType == StoryBeatType.NumRuns ){
                    // Separate from currentNumRunRemoveValue, if one has passed the threshold in which we're okay with seeing runNum dialogue, remove it
                    int opportunityPassedRemoveValue = 0;
                    int currentRunNumber = StoryManager.instance.currentRunNumber;
                    foreach(int num in NPC.ActiveNPC.hasNumRunDialogueList){
                        int difference = currentRunNumber - num;
                        // If THIS NPC has something to say about your number of runs within threshold runs, add it to the pool
                        if( difference <= numRunsThreshold && difference >= 0 ){
                            sortedStoryBeats.Add(beat);
                            currentNumRunRemoveValue = num;
                        }
                        else if( difference > numRunsThreshold ){
                            // If we've passed the threshold, remove it from the list so we stop looking for it
                            opportunityPassedRemoveValue = num;
                        }
                    }
                    NPC.ActiveNPC.hasNumRunDialogueList.Remove(opportunityPassedRemoveValue);
                }
                // If the beat type is lowHP, check conditions and maybe add it
                else if( beatType == StoryBeatType.LowHealth ){
                    StoryBeatLowHealth lowHPBeat = (StoryBeatLowHealth)beat;
                    // TODO: Change  MAX  to  CURRENT / MAX
                    if( playerStats.getMaxHitPoints() <= lowHPBeat.LowHealthThreshold() ){
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

            if( sortedStoryBeats.Max.GetBeatType() == StoryBeatType.NumRuns ){
                NPC.ActiveNPC.hasNumRunDialogueList.Remove(currentNumRunRemoveValue);
            }

            Debug.Log("PLAYING DIALOGUE INTERACTION for " + NPC.ActiveNPC.speakerData.SpeakerID() + ": " + sortedStoryBeats.Max.GetYarnHeadNode());

            // Return the highest priority beat
            return sortedStoryBeats.Max.GetYarnHeadNode();
        });


        // Add a function to tell yarn which node in that conditional branch to play
        dialogueRunner.AddFunction("SelectGenericNode", 1, delegate (Yarn.Value[] parameters){
            // Takes in a trigger; search the active NPC's dictionary for that key, the value is an int of where we're at
            StoryBeatType trigger = StoryBeatType.DefaultDialogue;
            for(int i = 0; i < (int)StoryBeatType.enumSize; ++i){
                if(( (StoryBeatType)i ).ToString().Equals(parameters[0])){
                    trigger = (StoryBeatType)i;
                }
            }
            // TODO: Find the current node position and return it

            // int currentNode = NPC.ActiveNPC.genericDialogueTriggers[trigger];   // Set the current position in the branch
            // NPC.ActiveNPC.genericDialogueTriggers[trigger] = currentNode + 1;   // Increment the value in the dictionary

            // TODO: If this condition is now out of interactions, remove it from the pool (NPC.ActiveNPC.RemoveDialogueTrigger(trigger))
            // Update: based on recent changes (aka the entire StoryManager + StoryBeat system) that might not be the way to do it in general
            // but yes we need some kind of BranchVisited function and stuff

            return 0;
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

    // Called in yarn scripts to set UI speaker info
    // [YarnCommand("SetSpeaker")]
    private void SetSpeakerUI(string[] info)
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

    // Called in yarn scripts to remove speakers from StoryBeats, in the final node of branches; called as  <<BranchComplete [beatType] [nodeNameBase]>>
    // Must pass in a beatType (identical to the enum string value) and a nodeName (without the speakerID or # in the branch, just the base) IN THAT ORDER
    // NEVER call this on repeatable type dialogue -> it should never be removed from the dialogue pool bc it's repeatable
    // [YarnCommand("BranchComplete")]
    public void BranchComplete(string[] info)
    {
        string beatTypeString = info[0];
        string nodeName = info[1];

        // Convert beat type string and find corresponding beat from node title
        StoryBeatType beatType = StoryManager.instance.GetBeatTypeFromString(beatTypeString);
        StoryBeat beat = StoryManager.instance.FindBeatFromNodeName(nodeName, beatType);

        Debug.Log("Removing " + beat.GetYarnHeadNode() + " from " + NPC.ActiveNPC.speakerData.SpeakerID());

        // Remove the speaker
        StoryManager.instance.RemoveSpeakerFromBeat(beat, NPC.ActiveNPC.speakerData.SpeakerID());
    }

    // Called by the Dialogue Runner to notify us that a node finished running
    public void NodeComplete(string nodeName)
    {
        // Log that the node has been run
        visitedNodes.Add(nodeName);
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