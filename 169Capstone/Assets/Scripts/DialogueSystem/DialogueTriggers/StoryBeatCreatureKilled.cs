using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatCreatureKilled")]
public class StoryBeatCreatureKilled : StoryBeat
{
    // Unique Trigger
    // [SerializeField] private EnemyStatObject enemy;     // The creature killed or killed by that can trigger dialogue
    [SerializeField] private EnemyID enemyID;

    [SerializeField] private bool playerKilledByCreature;   // True if the player was killed by this creature, False if the player killed this creature

    [Tooltip("JUST for adding to the yarn head node, if > 1")]
    [SerializeField] private int numCompletionsRequired = 1;

    public override void SetValues()
    {
        if(playerKilledByCreature){
            beatType = StoryBeatType.KilledBy;
        }
        else{
            beatType = StoryBeatType.EnemyKilled;
        }        

        yarnHeadNode = beatType.ToString() + enemyID;

        if(numCompletionsRequired > 1){
            yarnHeadNode = yarnHeadNode + numCompletionsRequired;
        }
    }

    // public EnemyStatObject GetEnemy()
    // {
    //     return enemy;
    // }

    public EnemyID GetEnemyID()
    {
        return enemyID;
    }
}
