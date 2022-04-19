using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/DropTable")]
public class DropTable : ScriptableObject
{
    #region DropTable Data Types

    [System.Serializable]
    public class DropTableEntry{
        [SerializeField] private InventoryItemSlot itemType;
        [SerializeField] private  ItemRarity rarity;
        [Tooltip("% chance of this item dropping at this tier")]
        [SerializeField] private float dropChance;

        public InventoryItemSlot ItemType(){return itemType;}
        public ItemRarity Rarity(){return rarity;}
        public float DropChance(){return dropChance;}
    }

    public struct ItemSlotRarityKey : System.IEquatable<ItemSlotRarityKey>
    {
        public InventoryItemSlot itemType;
        public ItemRarity rarity;

        public ItemSlotRarityKey(InventoryItemSlot _type, ItemRarity _rarity){
            itemType = _type;
            rarity = _rarity;
        }

        public override bool Equals(object obj) => obj is ItemSlotRarityKey other && this.Equals(other);

        public bool Equals(ItemSlotRarityKey other)
        {
            return itemType == other.itemType && rarity == other.rarity;
        }

        public override int GetHashCode() => ((int)itemType, (int)rarity).GetHashCode();
    }

    #endregion
    
    [SerializeField] private int bossesRequired;

    [SerializeField] private List<DropTableEntry> dropTableEntries = new List<DropTableEntry>();
    private Dictionary<ItemSlotRarityKey, float> dropTableDictionary = new Dictionary<ItemSlotRarityKey, float>();

    public Dictionary<ItemSlotRarityKey, float> DropTableDictionary()
    {
        if(dropTableDictionary.Count == 0){
            // Set up the dictionary if it hasn't been set up yet
            float cumulativeChanceValue = 0;
            foreach(DropTableEntry dte in dropTableEntries){
                cumulativeChanceValue += dte.DropChance();
                dropTableDictionary[ new ItemSlotRarityKey(dte.ItemType(), dte.Rarity()) ] = cumulativeChanceValue;
            }
            if(cumulativeChanceValue >= 1){
                Debug.LogWarning("Drop table for tier " + bossesRequired + " has a cumulative drop chance total of " + cumulativeChanceValue + "; some items will NEVER drop! Make sure drop chance total is < 1.");
            }
        }

        return dropTableDictionary;
    }

    public List<DropTableEntry> DropTableEntries()
    {
        return dropTableEntries;
    }
    
    public int BossesRequired()
    {
        return bossesRequired;
    }
}
