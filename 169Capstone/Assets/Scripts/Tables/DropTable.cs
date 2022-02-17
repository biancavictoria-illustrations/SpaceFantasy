using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/DropTableObject")]
public class DropTable : ScriptableObject
{
    [SerializeField] private List<InventoryItemSlot> itemType;
    [SerializeField] private List<ItemRarity> itemRarityTier;

    [Tooltip("% chance of item dropping, + all previous % entires. List of decimal values < 1, except the last value which must be 1 (cumulative total of all values).")]
    [SerializeField] private List<float> dropChance;

    [SerializeField] private int bossesRequired;
    

    public List<InventoryItemSlot> ItemType()
    {
        return itemType;
    }

    public List<ItemRarity> ItemRarityTier()
    {
        return itemRarityTier;
    }

    public List<float> DropChance()
    {
        return dropChance;
    }
}
