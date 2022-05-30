using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueEmoteType{
    angry,
    blush,
    happy,
    heart,
    sad,
    surprise,
    sweat,
    question,   // And exclamation point "?!"

    enumSize
}

public class DialogueEmote : MonoBehaviour
{
    [SerializeField] private DialogueEmoteType emoteType;

    private bool emoteIsActive = false;

    void Start()
    {
        DialogueManager.instance.dialogueUI.onLineEnd.AddListener(DeactivateOnLineComplete);
    }

    public DialogueEmoteType EmoteType()
    {
        return emoteType;
    }

    public void ToggleEmoteActive(bool set)
    {
        gameObject.SetActive(set);
        emoteIsActive = set;
    }

    private void DeactivateOnLineComplete()
    {
        if(emoteIsActive)
            ToggleEmoteActive(false);
    }
}
