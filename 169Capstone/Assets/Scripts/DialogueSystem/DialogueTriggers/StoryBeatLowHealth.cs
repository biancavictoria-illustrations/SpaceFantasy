using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatLowHealth")]
public class StoryBeatLowHealth : StoryBeat
{
    [SerializeField] float lowHealthThreshold = 0f;     // If currentHP / maxHP <= threshold, can play this dialogue

    void Awake()
    {
        beatType = StoryBeatType.lowHealth;
        priorityValue = DialoguePriority.p1;
    }

    public float LowHealthThreshold()
    {
        return lowHealthThreshold;
    }
}
