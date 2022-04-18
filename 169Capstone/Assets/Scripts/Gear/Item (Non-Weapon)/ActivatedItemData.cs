using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/ActivatedItemData")]
public class ActivatedItemData : ScriptableObject
{
    [SerializeField] private float coolDown;
    [SerializeField] private float damage;
    [SerializeField] private int radius;
    [SerializeField] private float duration;
    [SerializeField] private float range;

    public float CoolDown()
    {
        return coolDown;
    }

    public float Damage()
    {
        return damage;
    }

    public int Radius()
    {
        return radius;
    }

    public float Duration()
    {
        return duration;
    }

    public float Range()
    {
        return range;
    }
}
