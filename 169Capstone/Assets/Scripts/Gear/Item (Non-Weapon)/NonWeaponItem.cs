using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonWeaponItem : Equipment
{
    public GameObject timerPrefab;
    public ActivatedItemData itemData;
    [HideInInspector] public int slot;
    public bool fire = false;
    [HideInInspector] public bool clearToFire = true;

    [HideInInspector] public AnimationStateController anim;
    //[HideInInspector] public Timer timer;

    public void StartCooldown()
    {
        StartCoroutine("CoolDown");
    }
}
