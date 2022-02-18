using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all EquipmentBaseData in the game
[CreateAssetMenu(menuName = "Gear/GearManagerObject")]
public class GearManagerObject : ScriptableObject
{
    [SerializeField] private List<EquipmentBaseData> weapons;
    [SerializeField] private List<EquipmentBaseData> accessories;
    [SerializeField] private List<EquipmentBaseData> head;
    [SerializeField] private List<EquipmentBaseData> legs;

    public List<EquipmentBaseData> Weapons()
    {
        return weapons;
    }

    public List<EquipmentBaseData> Accessories()
    {
        return accessories;
    }

    public List<EquipmentBaseData> Head()
    {
        return head;
    }

    public List<EquipmentBaseData> Legs()
    {
        return legs;
    }
}
