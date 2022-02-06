using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EnemyLogicObject")]
public class EnemyLogic : ScriptableObject
{
    public float provokedRange;
    public float baseDamage;
    public List<AttackLogic> attacks;
}
