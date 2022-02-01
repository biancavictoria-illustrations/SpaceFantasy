using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all EquipmentData
[CreateAssetMenu(menuName = "Gear/GearManagerObject")]
public class GearManagerObject : ScriptableObject
{
    [SerializeField] private List<EquipmentData> weapons;
    [SerializeField] private List<EquipmentData> accessories;
    [SerializeField] private List<EquipmentData> head;
    [SerializeField] private List<EquipmentData> legs;

    public List<EquipmentData> Weapons()
    {
        return weapons;
    }

    public List<EquipmentData> Accessories()
    {
        return accessories;
    }

    public List<EquipmentData> Head()
    {
        return head;
    }

    public List<EquipmentData> Legs()
    {
        return legs;
    }
}
