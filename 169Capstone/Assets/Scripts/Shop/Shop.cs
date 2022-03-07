using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventory shopKeeper;
    private List<RarityAssignmentTier> inventoryList;
    private HashSet<int> indexes;

    public GameObject shopItemPrefab;

    public List<GeneratedEquipment> inventory {get; private set;}


    void Start()
    {
        GenerateShopInventory();
    }

    public void GenerateShopInventory()
    {
        indexes = new HashSet<int>();

        int tier = ObjectManager.bossesKilled;

        inventoryList = new List<RarityAssignmentTier>();
        inventory = new List<GeneratedEquipment>();

        inventoryList.Add(shopKeeper.Slot1());
        inventoryList.Add(shopKeeper.Slot2());
        inventoryList.Add(shopKeeper.Slot3());
        inventoryList.Add(shopKeeper.Slot4());
        inventoryList.Add(shopKeeper.Slot5());

        for(int i = 0; i < inventoryList.Count; i++){
            if(inventoryList[i] == RarityAssignmentTier.X){
                // Generate a new item with rarity = tier #, then add it to the inventory HashSet
                inventory.Add( GenerateItem((ItemRarity)tier) );
            }
            else if(inventoryList[i] == RarityAssignmentTier.X_1){
                inventory.Add( GenerateItem((ItemRarity)(tier+1)) );
            }
        }
    }

    private GeneratedEquipment GenerateItem(ItemRarity rarity)
    {
        // Generate a new (randomly selected) item with the determined rarity

        GameObject itemObject = Instantiate(shopItemPrefab);
        GeneratedEquipment generatedEquipment = itemObject.GetComponent<GeneratedEquipment>();
        generatedEquipment.SetEquipmentBaseData( PickItemFromPool(), rarity );

        // Debug.Log("Setting shop item to Rarity/ItemID: " + rarity + "/" + generatedEquipment.data.equipmentBaseData.ItemID());

        // TODO: Generate the item (see how it's done in the Drop script); for now, item will have default values besides it's EquipmentData and rarity

        /*
            Generating Drop Items:
            ======================
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
        */

        return generatedEquipment;
    }

    private EquipmentBaseData PickItemFromPool()
    {
        if(shopKeeper.Items().Count < 5){
            Debug.LogError("Shop contains less than 5 possible items in ShopKeeperInventory Item pool!");
            return shopKeeper.Items()[0];   // Default to returning just the first item, otherwise this will cause an infinite loop
        }

        // Randomly selects an item from the ShopKeeperInventory pool (no repeats)   
        int i = Random.Range(0,shopKeeper.Items().Count);
        while(indexes.Contains(i)){
            i = Random.Range(0,shopKeeper.Items().Count);
        }
        indexes.Add(i);

        return shopKeeper.Items()[i];      
    }
}
