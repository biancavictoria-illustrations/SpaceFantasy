using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool isInDialogue = false;

    public DialogueManager dialogueManager;

    public void Interact()
    {
        // If interact input
        if(Input.GetButtonDown("space")){
            // If NPC is active and not already talking
            if(NPC.ActiveNPC && !isInDialogue){
                StartDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        isInDialogue = true;
        dialogueManager.dialogueRunner.StartDialogue(NPC.ActiveNPC.yarnStartNode);
        // Start dialogue, activate UI -> don't let the player move anymore (pause the game, basically)
    }

    public void EndDialogue()
    {
        isInDialogue = false;
        // End dialogue, close UI -> let the player move (unpause the game)
    }
}
