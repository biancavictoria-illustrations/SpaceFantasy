using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : GeneratedEquipment
{
    public GameObject timerPrefab;
    public ItemObject itemObject;
    [HideInInspector] public int slot;
    public bool fire = false;
    [HideInInspector] public bool clearToFire = true;
    //[HideInInspector] public Timer timer;

    public IEnumerator CoolDown()
    {
        Timer timer = Instantiate(timerPrefab).GetComponent<Timer>();
        timer.StartTimer(itemObject.coolDown);
        yield return new WaitUntil(() => timer.timeRemaining <= 0);
        Destroy(timer);
        clearToFire = true;
    }
}
