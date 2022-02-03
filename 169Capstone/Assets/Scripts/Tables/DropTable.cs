using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/DropTableObject")]
public class DropTable : ScriptableObject
{
    [SerializeField] private List<InventoryItemSlot> itemType;
    [SerializeField] private List<ItemRarity> itemRarityTier;

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
