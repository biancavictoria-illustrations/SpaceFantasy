using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/DropObject")]
public class Drop : ScriptableObject
{
    [SerializeField] private List<DropTable> dropTables;
    [SerializeField] private LineTable lines;

    [SerializeField] private GameObject dropItemPrefab;


    // Called by the enemey health script when the enemy dies
    public void GetDrop(int tier, Transform pos)
    {
        // Select the drop table that coordinates with the current tier
        DropTable dropTable = dropTables[tier];

        // Generate a random number
        System.Random r = new System.Random();

        //float chance = (float)r.NextDouble();
        float chance = 0.89f; // for testing purposes

        // Determine item type and rarity
        int index = dropTable.DropChance().IndexOf(dropTable.DropChance().First(x => chance <= x));
        InventoryItemSlot itemType = dropTable.ItemType()[index];
        ItemRarity rarity = dropTable.ItemRarityTier()[index];
        
        EquipmentData item;
        List<ItemLine> secondaryLines;
        Debug.Log("Dropping " + itemType.ToString());

        // If the item is a weapon
        if(itemType == InventoryItemSlot.Weapon)
        {
            //GameObject test = Instantiate(emptyDropItem);
            //test.transform.position = pos.position;

            // Create item to drop in physical space from prefab
            GameObject dropItemObject = Instantiate(dropItemPrefab);
            dropItemObject.transform.position = pos.position;
            GeneratedEquipment generatedEquipment = dropItemObject.GetComponent<GeneratedEquipment>();

            // Find the EquipmentData and set it
            item = GameManager.instance.GearManager().Weapons()[r.Next(0, GameManager.instance.GearManager().Weapons().Count)];

            generatedEquipment.SetEquipmentData(item, rarity);
            Debug.Log("Setting drop item to Rarity/ItemID: " + rarity + "/" + item.ItemID());

            int i = lines.ItemType().IndexOf(item.ItemSlot());
            ItemLine primaryLine = lines.PrimaryWeaponLine()[i];
            float primaryLineTierScaling = 0.1f * tier;

            i = lines.ItemRarityTier().IndexOf(rarity);

            chance = (float)r.NextDouble();
            
            lines.Setup();

            int secondaryLineNum = lines.SecondaryLineNumberRates()[i].IndexOf(lines.SecondaryLineNumberRates()[i].First(x => chance <= x));
            chance = (float)r.NextDouble();
            int secondaryLineEnhancementsNum = lines.LineEnhancementRates()[i].IndexOf(lines.LineEnhancementRates()[i].First(x => chance <= x));    // Is this used anywhere?
            secondaryLines = GenerateSecondaryLines(secondaryLineNum);

            // Set the data in the generatedEquipment
            generatedEquipment.SetModifiers(primaryLine, primaryLineTierScaling, secondaryLines, tier);

            // Drop the item now that the data is generated!
            generatedEquipment.Drop();
        }
        else if(itemType == InventoryItemSlot.Accessory){
            // item = GameManager.instance.GearManager().Accessories()[r.Next(0, GameManager.instance.GearManager().Accessories().Count)];
        }
        else if(itemType == InventoryItemSlot.Helmet){
            // item = GameManager.instance.GearManager().Head()[r.Next(0, GameManager.instance.GearManager().Head().Count)];
        }
        else{
            // item = GameManager.instance.GearManager().Legs()[r.Next(0, GameManager.instance.GearManager().Legs().Count)];
        }

        // return $"{dropTable.ItemRarityTier[index]} {dropTable.ItemType[index]}";
    }

    private List<ItemLine> GenerateSecondaryLines(int size)
    {
        List<ItemLine> secondaryLines = new List<ItemLine>();
        System.Random r = new System.Random();

        for(int i = 0; i < size; i++){
            secondaryLines.Add(lines.LinePool()[r.Next(0, lines.LinePool().Count)]);
        }

        return secondaryLines;
    }

    public List<DropTable> DropTables()
    {
        return dropTables;
    }

    public LineTable Lines()
    {
        return lines;
    }
}
