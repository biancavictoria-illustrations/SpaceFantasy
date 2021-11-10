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

    public Dictionary<DialogueTrigger, int> genericDialogueTriggers = new Dictionary<DialogueTrigger, int>();   // Generic triggers and where we're at in those branches so far
    [SerializeField] private List<DialogueTrigger> genericDialogueTriggerList = new List<DialogueTrigger>();    // Every possible generic trigger this NPC has (not sure if this is gonna be necessary anymore... we'll see)

    public List<int> hasNumRunDialogueList = new List<int>();   // Times at which this NPC comments on how many runs you've done, in order

    void Start()
    {
        DialogueManager.instance.dialogueRunner.Add(yarnDialogue);
        DialogueManager.instance.AddSpeaker(speakerData);

        // Start/head node for a speaker's yarn file is always their unique speakerID + "Start"
        yarnStartNode = speakerData.SpeakerName() + "Start";

        // Set up the dictionary
        foreach(DialogueTrigger t in genericDialogueTriggerList){
            genericDialogueTriggers.Add(t,1);
        }

        // TODO: Check if this NPC has new dialogue to say; if so, they have a UI alert above their head
        // (set hasNewDialogue to true if the dialogue selected for them is anything BESIDES the super generic default repeatable category)
        // also you should only be able to talk to the NPC once and then you can't again til next run...
        hasNewDialogue = true;
        newDialogueAlert.SetActive(hasNewDialogue);
        // How does this work with scene transitions? How do we keep track of this stuff?
        // Should the dialogue/story manager just... hold all the NPCs' dialogue statuses, instead of the NPCs themselves?
    }

    // If all of the interactions for this trigger have been played, remove that trigger from the pool of options
    public void RemoveDialogueTrigger(DialogueTrigger usedTrigger)
    {
        if(genericDialogueTriggers.ContainsKey(usedTrigger)){
            genericDialogueTriggers.Remove(usedTrigger);
            return;
        }
        Debug.Log("Tried to remove dialogue trigger " + usedTrigger + " from " + speakerData.SpeakerName() + " but it doesn't exist!");
    }

    public void NewDialogueSpoken()
    {
        // TODO: Could just make this a function that takes in a bool and toggles the newdialoguealert on/off
        // unless the bool has another purpose?
        if(hasNewDialogue){
            hasNewDialogue = false;
            newDialogueAlert.SetActive(false);
        }
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
}
