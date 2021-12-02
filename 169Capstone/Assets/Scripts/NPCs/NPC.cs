using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{
    public static NPC ActiveNPC {get; private set;}

    [HideInInspector] public string yarnStartNode;      // Set on Start from the info in speakerData
    public YarnProgram yarnDialogue;
    public SpeakerData speakerData;

    public GameObject newDialogueAlert;

    public List<int> hasNumRunDialogueList = new List<int>();   // Times at which this NPC comments on how many runs you've done, in order


    void Start()
    {
        // If you haven't interacted with this NPC on this run yet, set interactable to true; otherwise, false
        SetNPCInteractable(!HaveTalkedToNPC());
        
        DialogueManager.instance.dialogueRunner.Add(yarnDialogue);
        DialogueManager.instance.AddSpeaker(speakerData);

        // Start/head node for a speaker's yarn file is always their unique speakerID + "Start"
        yarnStartNode = speakerData.SpeakerID() + "Start";
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

    // Toggles the newDialogueAlert on/off
    public void SetNPCInteractable(bool set)
    {
        newDialogueAlert.SetActive(set);
    }

    // When you finish dialogue, call this to deactivate the NPC
    public void TalkedToNPC()
    {
        SetNPCInteractable(false);

        SpeakerID speaker = speakerData.SpeakerID();
        if( speaker == SpeakerID.Bryn ){
            StoryManager.instance.talkedToBryn = true;
            return;
        }
        else if( speaker == SpeakerID.Andy ){
            StoryManager.instance.talkedToAndy = true;
            return;
        }
        else if( speaker == SpeakerID.Sorrel ){
            StoryManager.instance.talkedToSorrel = true;
            return;
        }
        else if( speaker == SpeakerID.Doctor ){
            StoryManager.instance.talkedToDoctor = true;
            return;
        }
        else if( speaker == SpeakerID.Stellan ){
            StoryManager.instance.talkedToStellan = true;
            return;
        }
        else if( speaker == SpeakerID.TimeLich ){
            StoryManager.instance.talkedToLich = true;
            return;
        }
        Debug.LogError("Tried to log talking to an NPC who does not exist! SpeakerID: " + speaker);
    }

    // Ask the story manager if we've talked to this NPC yet
    public bool HaveTalkedToNPC()
    {
        SpeakerID speaker = speakerData.SpeakerID();
        if( speaker == SpeakerID.Bryn ){
            return StoryManager.instance.talkedToBryn;
        }
        else if( speaker == SpeakerID.Andy ){
            return StoryManager.instance.talkedToAndy;
        }
        else if( speaker == SpeakerID.Sorrel ){
            return StoryManager.instance.talkedToSorrel;
        }
        else if( speaker == SpeakerID.Doctor ){
            return StoryManager.instance.talkedToDoctor;
        }
        else if( speaker == SpeakerID.Stellan ){
            return StoryManager.instance.talkedToStellan;
        }
        else if( speaker == SpeakerID.TimeLich ){
            return StoryManager.instance.talkedToLich;
        }
        Debug.LogError("Tried to check if you have talked to an NPC who does not exist! SpeakerID: " + speaker);
        return false;
    }
}
