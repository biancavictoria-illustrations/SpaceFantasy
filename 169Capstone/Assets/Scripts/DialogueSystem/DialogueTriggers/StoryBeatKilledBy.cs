using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatKilledBy")]
public class StoryBeatKilledBy : StoryBeat
{
    // Unique Trigger
    [SerializeField] private DamageSourceType damageSource;

    [Tooltip("JUST for adding to the yarn head node, if > 1")]
    [SerializeField] private int numCompletionsRequired = 1;

    public override void SetValues()
    {
        beatType = StoryBeatType.KilledBy;     

        yarnHeadNode = beatType.ToString() + damageSource;

        if(numCompletionsRequired > 1){
            yarnHeadNode = yarnHeadNode + numCompletionsRequired;
        }
    }

    public DamageSourceType DamageSource()
    {
        return damageSource;
    }
}
