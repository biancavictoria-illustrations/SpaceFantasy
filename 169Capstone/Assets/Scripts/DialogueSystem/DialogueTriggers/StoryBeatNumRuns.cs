using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatNumRuns")]
public class StoryBeatNumRuns : StoryBeat
{
    void Awake()
    {
        beatType = StoryBeatType.numRums;
        priorityValue = DialoguePriority.p2;
    }
}