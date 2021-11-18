using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatNumRuns")]
public class StoryBeatNumRuns : StoryBeat
{
    public override void SetValues()
    {
        beatType = StoryBeatType.NumRuns;
        priorityValue = DialoguePriority.p2;
        yarnHeadNode = beatType.ToString();
    }
}