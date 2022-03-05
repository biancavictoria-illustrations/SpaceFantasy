using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Narrative/StoryBeatConversation")]
public class StoryBeatConversation : StoryBeat
{    
    // Event Triggers

    [Tooltip("The NPC associated with this event trigger (NPC talked to)")]
    [SerializeField] private SpeakerID talkedToNPC;

    [Tooltip("The yarn head node for the dialogue branch completed (*** including NPC name ***)")]
    [SerializeField] private string otherDialogueHeadNode = "";

    public override void SetValues()
    {
        beatType = StoryBeatType.DialogueCompleted;
        yarnHeadNode = beatType + otherDialogueHeadNode;
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