using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemObject")]
public class ItemObject : ScriptableObject
{
    public float coolDown;
    public float damage;
    public int radius;
    public float duration;
    public float range;
}
