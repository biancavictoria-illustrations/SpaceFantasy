using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatLowHealth")]
public class StoryBeatLowHealth : StoryBeat
{
    [SerializeField] float lowHealthThreshold = 0f;     // If currentHP / maxHP <= threshold, can play this dialogue

    public override void SetValues()
    {
        beatType = StoryBeatType.LowHealth;
        priorityValue = DialoguePriority.p1;
        yarnHeadNode = beatType.ToString();
    }

    public float LowHealthThreshold()
    {
        return lowHealthThreshold;
    }
}
