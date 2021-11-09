using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Narrative/KilledDialogueTrigger")]
public class KilledDialogueTrigger : StoryBeat
{
    // Unique Trigger
    [SerializeField] private EnemyStatObject enemy;     // The creature killed or killed by that can trigger dialogue

    [SerializeField] private bool playerKilledByCreature;   // True if the player was killed by this creature, False if the player killed this creature

    void Awake()
    {
        if(playerKilledByCreature){
            beatType = StoryBeatType.killedBy;
            return;
        }
        beatType = StoryBeatType.creatureKilled;
    }

    public EnemyStatObject GetEnemy()
    {
        return enemy;
    }
}
