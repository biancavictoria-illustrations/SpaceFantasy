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
        DropTable dropTable = dropTables[tier];
        List<DropTable.DropTableEntry> dropTableEntries = dropTable.DropTableEntries();

        // Generate a random number, 0 - 1, to determine item type and rarity
        float chance = Random.Range(0.0f, 1f);

        float itemDropChanceValueFromTable = 0;  // The chance of this selected item dropping, as input in the drop table
        try{
            // TODO: Pick a random option from the pool instead of always taking the first match
            // itemDropChanceValueFromTable = dropTable.DropChance().First(x => chance <= x);
            itemDropChanceValueFromTable = dropTableEntries[0].DropChance();  // TEMP
        }
        catch{
            return;     // If the drop chance value is > all possible drop chance values in the list, don't drop anything
        }

        // int index = dropTable.DropChance().IndexOf(itemDropChanceValueFromTable);
        int index = 0;  // TEMP

        // InventoryItemSlot itemType = dropTable.ItemType()[index];
        // ItemRarity rarity = dropTable.ItemRarityTier()[index];

        InventoryItemSlot itemType = InventoryItemSlot.Weapon;  // TEMP
        ItemRarity rarity = ItemRarity.Common;  // TEMP
        
        EquipmentBaseData itemBaseData;

        // Get the equipment data for the right type of equipment
        if(itemType == InventoryItemSlot.Weapon)
        {
            itemBaseData = GameManager.instance.GearManager().Weapons()[Random.Range(0, GameManager.instance.GearManager().Weapons().Count)];
        }
        else if(itemType == InventoryItemSlot.Accessory){
            itemBaseData = GameManager.instance.GearManager().Accessories()[Random.Range(0, GameManager.instance.GearManager().Accessories().Count)];
        }
        else if(itemType == InventoryItemSlot.Helmet){
            itemBaseData = GameManager.instance.GearManager().Head()[Random.Range(0, GameManager.instance.GearManager().Head().Count)];
        }
        else{   // If boots/legs
            itemBaseData = GameManager.instance.GearManager().Legs()[Random.Range(0, GameManager.instance.GearManager().Legs().Count)];
        }

        // Create item to drop in physical space from prefab
        GameObject dropItemObject = Instantiate(dropItemPrefab);
        dropItemObject.transform.position = pos.position;
        GeneratedEquipment generatedEquipment = dropItemObject.GetComponent<GeneratedEquipment>();

        // Use the static item generator to generate the lines and stuff from here
        GenericItemGenerator.GenerateItem(ref generatedEquipment, itemBaseData, rarity, enemyLineTable);

        // Drop the item now that the data is generated!
        generatedEquipment.GetComponent<DropTrigger>().DropItemModelIn3DSpace();
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
