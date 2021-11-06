using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public SpeakerData speakerData;
    private bool isInDialogue = false;

    void Start()
    {
        DialogueManager.instance.AddSpeaker(speakerData);
    }

    void Update()
    {
        // If interact input
        if(Input.GetKeyDown(KeyCode.Space)){
            // If NPC is active and not already talking
            if(NPC.ActiveNPC && !isInDialogue){
                StartDialogue();
            }
        }
    }

    // Called when the player clicks the interact button in range of an NPC with something to say
    private void StartDialogue()
    {
        // pause player movement (add this stuff to the normal player script -> start of Update: if(isindialogue){return};)
        isInDialogue = true;
        DialogueManager.instance.dialogueRunner.StartDialogue(NPC.ActiveNPC.yarnStartNode);
    }

    // Called when the dialogue ends/closes
    public void EndDialogue()
    {
        // unpause player movement
        isInDialogue = false;
        NPC.ActiveNPC.NewDialogueSpoken();
    }
}
