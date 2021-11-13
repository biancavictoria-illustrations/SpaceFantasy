using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatDefault")]
public class StoryBeatDefault : StoryBeat
{
    public override void SetValues()
    {
        beatType = StoryBeatType.DefaultDialogue;
        priorityValue = DialoguePriority.p1;
        yarnHeadNode = beatType.ToString();
    }
}
