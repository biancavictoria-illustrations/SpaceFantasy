using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatRepeatableGeneric")]
public class StoryBeatRepeatableGeneric : StoryBeat
{
    void Awake()
    {
        beatType = StoryBeatType.repeatableGeneric;
    }
}
