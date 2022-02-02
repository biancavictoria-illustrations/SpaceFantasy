using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/ShopKeeperInventoryObject")]
public class ShopKeeperInventory : ScriptableObject
{
    [SerializeField] private string slot1;
    [SerializeField] private string slot2;
    [SerializeField] private string slot3;
    [SerializeField] private string slot4;
    [SerializeField] private string slot5;

    // All possible items this shopkeeper could carry, set in the inspector
    // CONSDIER: This could be just their ItemIDs instead to make it lighter weight??? Idk
    [SerializeField] private List<EquipmentData> items = new List<EquipmentData>();


    public string Slot1()
    {
        return slot1;
    }

    public string Slot2()
    {
        return slot2;
    }

    public string Slot3()
    {
        return slot3;
    }

    public string Slot4()
    {
        return slot4;
    }

    public string Slot5()
    {
        return slot5;
    }

    public List<EquipmentData> Items()
    {
        return items;
    }
}
