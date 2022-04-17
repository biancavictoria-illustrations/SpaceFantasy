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

        // Generate a random number
        // float chance = Random.Range(0.0f, 1f);

        // Determine item type and rarity
        // DOESN'T WORK for some reason
        // int index = dropTable.DropChance().IndexOf(dropTable.DropChance().First(x => chance <= x));

        // TEMP get a random thing (not based on drop chance values, just a random index)
        // TODO: Drop chance
        int index = Random.Range(0, dropTable.DropChance().Count);

        InventoryItemSlot itemType = dropTable.ItemType()[index];
        ItemRarity rarity = dropTable.ItemRarityTier()[index];
        
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
