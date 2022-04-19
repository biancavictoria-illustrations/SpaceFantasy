using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/DropObject")]
public class EnemyDropGenerator : ScriptableObject
{
    [SerializeField] private List<DropTable> dropTables;
    [SerializeField] private LineTable enemyLineTable;

    [SerializeField] private GameObject dropItemPrefab;

    // Called by the enemey health script when the enemy dies
    public void GetDrop(int tier, Transform pos)
    {
        // Select the drop table that coordinates with the current tier
        DropTable dropTableObjectForTier = dropTables[tier];
        Dictionary<DropTable.ItemSlotRarityKey, float> dropTable = dropTableObjectForTier.DropTableDictionary();

        // Generate a random number, 0 - 1, to determine item type and rarity
        float chance = Random.Range(0.0f, 1f);
        InventoryItemSlot itemType = InventoryItemSlot.enumSize;
        ItemRarity rarity = ItemRarity.enumSize;

        float itemDropChanceValueFromTable = 0;  // The chance of this selected item dropping, as input in the drop table
        try{
            // Get the first match for an item with this drop chance (bc values are cumulative, this won't return the same thing every time)
            KeyValuePair<DropTable.ItemSlotRarityKey, float> matchEntry = dropTable.First( entry => chance <= entry.Value );
            itemDropChanceValueFromTable = matchEntry.Value;
            itemType = matchEntry.Key.itemType;
            rarity = matchEntry.Key.rarity;
        }
        catch{
            return;     // If the drop chance value is > all possible drop chance values in the list, don't drop anything
        }
        
        // Get the equipment data for the right type of equipment
        EquipmentBaseData equipmentBaseData = GetEquipmentBaseData(itemType);

        // Create item to drop in physical space from prefab
        GameObject dropItemObject = Instantiate(dropItemPrefab);
        dropItemObject.transform.position = pos.position;
        GeneratedEquipment generatedEquipment = dropItemObject.GetComponent<GeneratedEquipment>();

        // Use the static item generator to generate the lines and stuff from here
        GenericItemGenerator.GenerateItem(ref generatedEquipment, equipmentBaseData, rarity, enemyLineTable);

        // Drop the item now that the data is generated!
        generatedEquipment.GetComponent<DropTrigger>().DropItemModelIn3DSpace();
    }

    private bool DropChanceSatisfied(float calculatedChance, float entryDropChance)
    {
        if(calculatedChance <= entryDropChance){
            return true;
        }
        return false;
    }

    private EquipmentBaseData GetEquipmentBaseData(InventoryItemSlot itemType)
    {
        if(itemType == InventoryItemSlot.Weapon)
        {
            return GameManager.instance.GearManager().Weapons()[Random.Range(0, GameManager.instance.GearManager().Weapons().Count)];
        }
        else if(itemType == InventoryItemSlot.Accessory){
            return GameManager.instance.GearManager().Accessories()[Random.Range(0, GameManager.instance.GearManager().Accessories().Count)];
        }
        else if(itemType == InventoryItemSlot.Helmet){
            return GameManager.instance.GearManager().Head()[Random.Range(0, GameManager.instance.GearManager().Head().Count)];
        }
        else{   // If boots/legs
            return GameManager.instance.GearManager().Legs()[Random.Range(0, GameManager.instance.GearManager().Legs().Count)];
        }
    }

    public List<DropTable> DropTables()
    {
        return dropTables;
    }

    public LineTable Lines()
    {
        return enemyLineTable;
    }
}
