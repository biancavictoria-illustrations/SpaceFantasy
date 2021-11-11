using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatDefault")]
public class StoryBeatDefault : StoryBeat
{
    void Awake()
    {
        beatType = StoryBeatType.defaultDialogue;
        priorityValue = DialoguePriority.p1;
    }
}
