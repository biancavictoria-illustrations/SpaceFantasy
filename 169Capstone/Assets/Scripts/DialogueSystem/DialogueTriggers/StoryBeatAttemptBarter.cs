using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Narrative/StoryBeatAttemptBarter")]
public class StoryBeatAttemptBarter : StoryBeat
{
    [SerializeField] private bool isSuccess;

    void Awake()
    {
        priorityValue = DialoguePriority.p1;
        if(isSuccess){
            beatType = StoryBeatType.barterSuccess;
            return;
        }
        beatType = StoryBeatType.barterFail;
    }
}
