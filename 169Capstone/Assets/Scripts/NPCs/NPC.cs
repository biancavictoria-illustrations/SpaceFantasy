using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public static NPC ActiveNPC {get; private set;}

    public string characterName;

    public string yarnStartNode = "Start";
    public YarnProgram yarnDialogue;

    public DialogueManager dialogueManager;

    public bool NPCisActive = false;

    void Start()
    {
        dialogueManager.dialogueRunner.Add(yarnDialogue);

        // TEMP (cut this after testing)
        ActiveNPC = true ? this : null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player is near this NPC
        // if()
        // {
        //     ActiveNPC = true ? this : null;
        // }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player is no longer near this NPC
        // if()
        // {
        //     ActiveNPC = false ? this : null;
        // }
    }
}
