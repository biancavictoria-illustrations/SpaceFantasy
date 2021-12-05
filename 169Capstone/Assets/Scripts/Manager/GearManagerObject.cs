using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GearManagerObject")]
public class GearManagerObject : ScriptableObject
{
    public GameObject[] weapons;
    public GameObject[] accessories;
    public GameObject[] head;
    public GameObject[] legs;
}
