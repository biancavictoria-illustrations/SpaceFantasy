using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{   
    // Make this a singleton so that it can be accessed from anywhere and there's only one
    public static DialogueManager instance;

    public GameObject dialogueUICanvas;

    private Dictionary<string, SpeakerData> speakers = new Dictionary<string, SpeakerData>();

    public Image characterPortrait;
    public TMP_Text speakerName;

    public DialogueRunner dialogueRunner;
    // Keeps track of what nodes the player has seen so that we don't see those again
    private HashSet<string> visitedNodes = new HashSet<string>();

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }

        // Add SetSpeakerInfo to yarn so that we can set character portraits and names
        dialogueRunner.AddCommandHandler("SetSpeaker", SetSpeakerInfo);

        // Add Visited to yarn so that we can check if a node has been visited yet
        dialogueRunner.AddFunction("Visited", 1, delegate (Yarn.Value[] parameters){
            var nodeName = parameters[0];
            return visitedNodes.Contains(nodeName.AsString);
        });

        DontDestroyOnLoad(this.gameObject);
        // ... right?
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

    /*
        TODO: Make functions to send to Yarn as commands to use to select which dialogue to play

        Like depending on certain conditions, automatically go to different nodes (based on a set
        priority order)

        === Possible Node/Conditions ===
        - default
        - low HP
        - # runs > ?
        - has [item]
        - has [item] from [NPC]
        - stat values
        - high CHA (successful barter)
        - low CHA (unsuccessful barter)
        - beat a boss
        - lost to a boss

        Condition node could be the starting place, and then it branches off into a variety
        of sub-nodes from there. Each of those nodes can then be checked like "visited lowHP2" and it
        could go in order there

        So priority/some randomness selection for which condition node to access, and then goes linearly
        down that branch each time you visit that condition node

        Should probably keep track of where you're at in each tree HERE and then just tell Yarn that info?

        Like "yarn go to CONDITION [lowHP], NODE [4]" for the fourth low HP interaction
    */

}
