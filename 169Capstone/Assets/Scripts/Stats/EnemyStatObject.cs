using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unique internal IDs per enemy
public enum EnemyID
{
    TimeLich,
    Slime,
    EnchantedCloak,
    enumSize
}

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyStatObject")]
public class EnemyStatObject : ScriptableObject
{
    [SerializeField] private EnemyID enemyID;

    [SerializeField] private float maxHitPoints;    
    [SerializeField] private float attackSpeed; 
    [SerializeField] private float moveSpeed;   
    [SerializeField] private float defense; 
    [SerializeField] private float dodgeChance; 
    [SerializeField] private float critChance;  
    [SerializeField] private float critDamage;  
    [SerializeField] private float stunChance;  
    [SerializeField] private float burnChance;
    [SerializeField] private float slowChance; 
    [SerializeField] private float statusResistChance;

    public EnemyID EnemyID()
    {
        return enemyID;
    }

    public float MaxHitPoints()
    {
        return maxHitPoints;
    }

    public float AttackSpeed()
    {
        return attackSpeed;
    }

    public float MoveSpeed()
    {
        return moveSpeed;
    }

    public float Defense()
    {
        return defense;
    }

    public float DodgeChance()
    {
        return dodgeChance;
    }

    public float CritChance()
    {
        return critChance;
    }

    public float CritDamage()
    {
        return critDamage;
    }

    public float StunChance()
    {
        return stunChance;
    }

    public float BurnChance()
    {
        return burnChance;
    }

    public float SlowChance()
    {
        return slowChance;
    }

    public float StatusResistChance()
    {
        return statusResistChance;
    }
}
