using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO

// maybe just revamp attempt barter cuz they need the same stuff? just give it a stat value as well
// if CHA, treat it as barter attempts currently are

[CreateAssetMenu(menuName = "Narrative/StoryBeatStatValue")]
public class StoryBeatStatValue : StoryBeat
{
    [SerializeField] private bool isSuccess;

    public override void SetValues()
    {
        // if(isSuccess){
        //     beatType = StoryBeatType.BarterSuccess;
        // }
        // else{
        //     beatType = StoryBeatType.BarterFail;
        // }

        // yarnHeadNode = beatType.ToString();
    }
}
