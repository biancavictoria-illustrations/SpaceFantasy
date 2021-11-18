using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Narrative/StoryBeatAttemptBarter")]
public class StoryBeatAttemptBarter : StoryBeat
{
    [SerializeField] private bool isSuccess;

    public override void SetValues()
    {
        priorityValue = DialoguePriority.p1;

        if(isSuccess){
            beatType = StoryBeatType.BarterSuccess;
        }
        else{
            beatType = StoryBeatType.BarterFail;
        }

        yarnHeadNode = beatType.ToString();
    }
}
