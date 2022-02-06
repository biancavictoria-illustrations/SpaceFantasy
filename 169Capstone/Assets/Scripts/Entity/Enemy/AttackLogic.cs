using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AttackLogicObject")]
public class AttackLogic : ScriptableObject
{
    public float attackRange;
    public float damageMultiplier = 1;
    public float windUp;
    public float duration;
    public float windDown;
    public float coolDown;
}
