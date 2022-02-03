using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Narrative/StoryBeatCreatureKilled")]
public class StoryBeatCreatureKilled : StoryBeat
{
    // Unique Trigger
    [SerializeField] private EnemyStatObject enemy;     // The creature killed or killed by that can trigger dialogue
    [SerializeField] private EnemyID enemyID;

    [SerializeField] private bool playerKilledByCreature;   // True if the player was killed by this creature, False if the player killed this creature

    public override void SetValues()
    {
        if(playerKilledByCreature){
            beatType = StoryBeatType.KilledBy;
            return;
        }
        beatType = StoryBeatType.EnemyKilled;

        yarnHeadNode = beatType.ToString() + enemyID;
    }

    public EnemyStatObject GetEnemy()
    {
        return enemy;
    }

    public EnemyID GetEnemyID()
    {
        return enemyID;
    }
}
