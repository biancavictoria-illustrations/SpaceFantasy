using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyStatObject")]
public class EnemyStatObject : ScriptableObject
{
    public float maxHitPoints;// {get; private set;}
    public float attackSpeed;// {get; private set;}
    public float moveSpeed;// {get; private set;}
    public float defense;// {get; private set;}
    public float dodgeChance;// {get; private set;}
    public float critChance;// {get; private set;}
    public float critDamage;// {get; private set;}
    public float stunChance;// {get; private set;}
    public float burnChance;// {get; private set;}
    public float slowChance;// {get; private set;}
    public float statusResistChance;// {get; private set;}

}
