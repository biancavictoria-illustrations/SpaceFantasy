using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Narrative/StoryBeatConversation")]
public class StoryBeatConversation : StoryBeat
{    
    // Event Triggers

    [Tooltip("The EXACT yarn node for the dialogue completed (*** including NPC name ***); IGNORE PREREQS do this instead; THIS node will have a head node of DialogueCompleted + this")]
    [SerializeField] private string completedDialogueNode;

    public override void SetValues()
    {
        beatType = StoryBeatType.DialogueCompleted;

        yarnHeadNode = beatType + completedDialogueNode;
    }

    public string CompletedDialogueNode()
    {
        return completedDialogueNode;
    }
}