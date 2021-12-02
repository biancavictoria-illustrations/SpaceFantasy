using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject timerPrefab;
    public ItemObject itemObject;
    [HideInInspector] public int slot;
    [HideInInspector] public bool fire = false;
    [HideInInspector] public Timer timer;
}
