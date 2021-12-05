using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/TitleManagerObject")]
public class TitleManagerObject : ScriptableObject
{
    public string[] weapons;
    public string[] accessories;
    public string[] head;
    public string[] legs;
}
