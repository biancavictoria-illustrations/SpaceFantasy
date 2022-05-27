using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RarityAssignmentTier{
    X,
    X_1
}

[CreateAssetMenu(menuName = "Gear/ShopKeeperInventoryObject")]
public class ShopKeeperInventory : ScriptableObject
{
    [SerializeField] private RarityAssignmentTier slot1;
    [SerializeField] private RarityAssignmentTier slot2;
    [SerializeField] private RarityAssignmentTier slot3;
    [SerializeField] private RarityAssignmentTier slot4;
    [SerializeField] private RarityAssignmentTier slot5;

    // All possible items this shopkeeper could carry, set in the inspector
    [SerializeField] private List<EquipmentBaseData> items = new List<EquipmentBaseData>();


    public RarityAssignmentTier Slot1()
    {
        return slot1;
    }

    public RarityAssignmentTier Slot2()
    {
        return slot2;
    }

    public RarityAssignmentTier Slot3()
    {
        return slot3;
    }

    public RarityAssignmentTier Slot4()
    {
        return slot4;
    }

    public RarityAssignmentTier Slot5()
    {
        return slot5;
    }

    public List<EquipmentBaseData> Items()
    {
        return items;
    }
}
