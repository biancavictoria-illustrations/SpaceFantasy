using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{
    public static NPC ActiveNPC {get; private set;}

    [HideInInspector] public string yarnStartNode;      // Set on Start from the info in speakerData
    public YarnProgram yarnDialogue;
    public SpeakerData speakerData;

    [HideInInspector] public bool hasNewDialogue;
    public GameObject newDialogueAlert;

    public List<int> hasNumRunDialogueList = new List<int>();   // Times at which this NPC comments on how many runs you've done, in order

    void Start()
    {
        hasNewDialogue = true;
        
        DialogueManager.instance.dialogueRunner.Add(yarnDialogue);
        DialogueManager.instance.AddSpeaker(speakerData);

        // Start/head node for a speaker's yarn file is always their unique speakerID + "Start"
        yarnStartNode = speakerData.SpeakerID() + "Start";

        // TODO: You should only be able to talk to the NPC once and then you can't again til next run...
        // once you've talked to them, you can't again!
        newDialogueAlert.SetActive(hasNewDialogue);
    }

    // Toggles the newDialogueAlert on/off
    public void HasNewDialogue(bool set)
    {
        hasNewDialogue = set;
        newDialogueAlert.SetActive(set);
    }

    // When player gets within range, they can start dialogue
    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveNPC = this;
        }
    }

    // When player leaves the range, they can no longer start the dialogue
    private void OnTriggerExit(Collider other)
    {
        // If the player is no longer near this NPC
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveNPC = null;
        }
    }

    // After a player talks to them once, they can't again this run
    // TODO: Call this after a player interacts with this NPC on a given run! (also write this lol)
    // There MIGHT be a way to do this with yarn spinner...........
    // Like it tells the dialogue manager who you talked to and the dialogue manager says great no more talking to that person i forbid it
    public void NotInteractable()
    {
        // Could take in a bool set and toggle NPC interactability, if we need that functionality
    }
}
