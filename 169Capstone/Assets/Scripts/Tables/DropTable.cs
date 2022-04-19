using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class DropTableEntry{
        [SerializeField] private InventoryItemSlot itemType;
        [SerializeField] private  ItemRarity rarity;
        [Tooltip("% chance of item dropping, + all previous % entires. List of decimal values < 1, except the last value which must be 1 (cumulative total of all values).")]
        [SerializeField] private float dropChance;

        public InventoryItemSlot ItemType(){return itemType;}
        public ItemRarity Rarity(){return rarity;}
        public float DropChance(){return dropChance;}
    }

    [SerializeField] private List<DropTableEntry> dropTableEntries;

    [SerializeField] private int bossesRequired;
    

    public List<DropTableEntry> DropTableEntries()
    {
        return dropTableEntries;
    }
    
    public int BossesRequired()
    {
        return bossesRequired;
    }
}
