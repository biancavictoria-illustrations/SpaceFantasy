using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Narrative/ConversationTrigger")]
public class ConversationTrigger : StoryBeat
{    
    // Event Triggers
    [SerializeField] private SpeakerID talkedToNPC;         // The NPC associated with this event trigger (NPC talked to)
    [SerializeField] private string otherDialogueHeadNode = "";     // The yarn head node for the dialogue (branch?) completed

    void Awake()
    {
        beatType = StoryBeatType.dialogueCompleted;
    }

    public SpeakerID GetTalkedToNPC()
    {
        return talkedToNPC;
    }

    public string GetOtherDialogue()
    {
        return otherDialogueHeadNode;
    }
}