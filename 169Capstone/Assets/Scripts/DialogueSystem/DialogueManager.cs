using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    // TODO: Add speaker sprite
    // public Sprite characterImage;

    public TMP_Text speakerName;
    public TMP_Text dialogue;

    public DialogueRunner dialogueRunner;

    public GameObject dialogueUIPanel;

    void Start()
    {
        
    }

    public void MarkLineComplete()
    {

    }

    public void ShowDialogueUI()
    {
        dialogueUIPanel.SetActive(true);
    }

    public void HideDialogueUI()
    {
        dialogueUIPanel.SetActive(false);
    }

    // public void StartDialogue()
    // {
    //     // isInDialogue = true;
    //     // Start dialogue, activate UI -> don't let the player move anymore (pause the game, basically)
    // }

    // public void EndDialogue()
    // {
    //     // isInDialogue = false;
    //     // End dialogue, close UI -> let the player move (unpause the game)
    // }
}
