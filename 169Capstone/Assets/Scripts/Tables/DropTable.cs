using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DropTableObject")]
public class DropTable : ScriptableObject
{
    public List<string> ItemType;
    public List<string> ItemRarityTier;
    public List<float> DropChance;
    [SerializeField] private int bossesRequired;
}
