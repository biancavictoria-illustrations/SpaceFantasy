using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatCreatureKilled")]
public class StoryBeatCreatureKilled : StoryBeat
{
    // Unique Trigger
    [SerializeField] private EnemyID enemyID;

    [Tooltip("JUST for adding to the yarn head node, if > 1")]
    [SerializeField] private int numCompletionsRequired = 1;

    public override void SetValues()
    {
        beatType = StoryBeatType.EnemyKilled;      

        yarnHeadNode = beatType.ToString() + enemyID;

        if(numCompletionsRequired > 1){
            yarnHeadNode = yarnHeadNode + numCompletionsRequired;
        }
    }

    public EnemyID GetEnemyID()
    {
        return enemyID;
    }
}
