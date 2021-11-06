using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public static NPC ActiveNPC {get; private set;}

    public string yarnStartNode = "Start";
    public YarnProgram yarnDialogue;

    public SpeakerData speakerData;

    [HideInInspector] public bool hasNewDialogue;
    public GameObject newDialogueAlert;

    void Start()
    {
        DialogueManager.instance.dialogueRunner.Add(yarnDialogue);
        DialogueManager.instance.AddSpeaker(speakerData);

        // TODO: Check if this NPC has new dialogue to say; if so, they have a UI alert above their head
        // also you should only be able to talk to the NPC once and then you can't again til next run
        hasNewDialogue = true;
        newDialogueAlert.SetActive(hasNewDialogue);
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
