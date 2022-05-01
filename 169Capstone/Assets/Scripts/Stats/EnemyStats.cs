using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    public EnemyStatObject stats;// {get; private set;}
    public EnemyID enemyID {get; private set;}

    public void initializeStats()
    {
        maxHitPointsBase = stats.MaxHitPoints();
        attackSpeedBase = stats.AttackSpeed();
        moveSpeedBase = stats.MoveSpeed();
        defenseBase = stats.Defense();
        dodgeChanceBase = stats.DodgeChance();
        critChanceBase = stats.CritChance();
        critDamageBase = stats.CritDamage();
        trapDamageResistBase = stats.TrapDamageResist();

        maxHitPointsMultiplier = 1;
        attackSpeedMultiplier = 1;
        moveSpeedMultiplier = 1;
        defenseMultiplier = 1;

        enemyID = stats.EnemyID();
    }
}
