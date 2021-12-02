using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyLogicObject")]
public class EnemyLogic : ScriptableObject
{
    public float provokedRange;
    public float attackRange;
    public float damage;
    public float windUp;
    public float duration;
    public float windDown;
    public float coolDown;
}
