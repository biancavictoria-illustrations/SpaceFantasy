using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventory shopKeeper;
    private List<string> inventoryList;
    private HashSet<int> indexes;

    public HashSet<GeneratedEquipment> inventory {get; private set;}


    void Start()
    {
        GenerateShopInventory();
    }

    public void GenerateShopInventory()
    {
        indexes = new HashSet<int>();

        int tier = ObjectManager.bossesKilled;

        inventoryList = new List<string>();
        inventory = new HashSet<GeneratedEquipment>();

        inventoryList.Add(shopKeeper.Slot1());
        inventoryList.Add(shopKeeper.Slot2());
        inventoryList.Add(shopKeeper.Slot3());
        inventoryList.Add(shopKeeper.Slot4());
        inventoryList.Add(shopKeeper.Slot5());

        for(int i = 0; i < inventoryList.Count; i++){
            if(inventoryList[i].Contains("[tier x]")){
                // Generate a new item with rarity = tier #, then add it to the inventory HashSet
                inventory.Add( GenerateItem((ItemRarity)tier) );
            }
            else if(inventoryList[i].Contains("[tier x+1]")){
                inventory.Add( GenerateItem((ItemRarity)(tier+1)) );
            }
        }
    }

    private GeneratedEquipment GenerateItem(ItemRarity rarity)
    {
        // Generate a new (randomly selected) item with the determined rarity
        GeneratedEquipment item = new GeneratedEquipment();
        item.SetEquipmentData( PickItemFromPool(), rarity );

        // TODO: Generate the item (see how it's done in the Drop script); for now, item will have default values besides it's EquipmentData and rarity

        return item;
    }

    private EquipmentData PickItemFromPool()
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
