using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventory shopKeeper;
    private List<RarityAssignmentTier> inventoryList;
    private HashSet<int> indexes;

    [SerializeField] private LineTable lines;
    [SerializeField] private GameObject shopItemPrefab;

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

        // TEMP: Only generates WEAPONS (remove once other gear is implemented)
        if( generatedEquipment.equipmentBaseData.ItemSlot() == InventoryItemSlot.Weapon ){
            int i = lines.ItemType().IndexOf(generatedEquipment.equipmentBaseData.ItemSlot());
            StatType primaryLine = lines.PrimaryWeaponLine()[i];
            float primaryLineTierScaling = 0.1f * (int)rarity;

            i = lines.ItemRarityTier().IndexOf(rarity);

            // Generate a random number
            System.Random r = new System.Random();
            float chance = (float)r.NextDouble();
            chance = (float)r.NextDouble();
            
            lines.Setup();

            int secondaryLineNum = lines.SecondaryLineNumberRates()[i].IndexOf(lines.SecondaryLineNumberRates()[i].First(x => chance <= x));
            chance = (float)r.NextDouble();
            int secondaryLineEnhancementsNum = lines.LineEnhancementRates()[i].IndexOf(lines.LineEnhancementRates()[i].First(x => chance <= x));    // Is this used anywhere?
            List<StatType> secondaryLines = GenerateSecondaryLines(secondaryLineNum);

            // Set the data in the generatedEquipment
            generatedEquipment.SetModifiers(primaryLine, primaryLineTierScaling, secondaryLines, (int)rarity);
        }       

        return generatedEquipment;
    }

    private List<StatType> GenerateSecondaryLines(int size)
    {
        List<StatType> secondaryLines = new List<StatType>();
        System.Random r = new System.Random();

        for(int i = 0; i < size; i++){
            secondaryLines.Add(lines.LinePool()[r.Next(0, lines.LinePool().Count)]);
        }

        return secondaryLines;
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
