using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    public EnemyStatObject stats;// {get; private set;}

    public void initializeStats()
    {
        maxHitPointsBase = stats.maxHitPoints;
        attackSpeedBase = stats.attackSpeed;
        moveSpeedBase = stats.moveSpeed;
        defenseBase = stats.defense;
        dodgeChanceBase = stats.dodgeChance;
        critChanceBase = stats.critChance;
        critDamageBase = stats.critDamage;
        stunChanceBase = stats.stunChance;
        burnChanceBase = stats.burnChance;
        slowChanceBase = stats.slowChance;
        statusResistChanceBase = stats.statusResistChance;

        maxHitPointsMultiplier = 1;
        attackSpeedMultiplier = 1;
        moveSpeedMultiplier = 1;
        defenseMultiplier = 1;
        dodgeChanceMultiplier = 1;
        critChanceMultiplier = 1;
        critDamageMultiplier = 1;
        stunChanceMultiplier = 1;
        burnChanceMultiplier = 1;
        slowChanceMultiplier = 1;
        statusResistChanceMultiplier = 1;
    }
}
