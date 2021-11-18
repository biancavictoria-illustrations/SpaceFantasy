using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatRepeatableGeneric")]
public class StoryBeatRepeatableGeneric : StoryBeat
{
    public override void SetValues()
    {
        beatType = StoryBeatType.Repeatable;
        priorityValue = DialoguePriority.minRepeatable;
        yarnHeadNode = beatType.ToString();
    }
}
