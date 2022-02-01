using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventory shopKeeper;
    private List<string> inventoryList;

    public HashSet<EquipmentData> inventory {get; private set;}


    void Start()
    {
        GenerateShopInventory();
    }

    private void GenerateShopInventory()
    {
        int tier = ObjectManager.bossesKilled;

        inventoryList = new List<string>();
        inventory = new HashSet<EquipmentData>();

        inventoryList.Add(shopKeeper.Slot1());
        inventoryList.Add(shopKeeper.Slot2());
        inventoryList.Add(shopKeeper.Slot3());
        inventoryList.Add(shopKeeper.Slot4());
        inventoryList.Add(shopKeeper.Slot5());

        for(int i = 0; i < inventoryList.Count; i++)
        {
            if(inventoryList[i].Contains("[tier x]"))
            {
                // Generate a new item with rarity = tier #, then add it to the inventory HashSet
                inventory.Add( PickItemFromPool((ItemRarity)tier) );
            }
            else if(inventoryList[i].Contains("[tier x+1]"))
            {
                inventory.Add( PickItemFromPool((ItemRarity)(tier+1)) );
            }
        }
    }

    // Pass in anything required and generate just the data, not the full item objects
    private EquipmentData PickItemFromPool(ItemRarity rarity)
    {
        // TODO: Randomly selects 5 items from the ShopKeeperInventory pool (no repeats, presumably?) (and generates necessary data)
        // temp: just picks the first one
        EquipmentData data = shopKeeper.Items()[0];
        return data;        
    }

    // TODO: Pass in anything else required; also this shouldn't be in shop, this should be generalized and also usable by drops
    public EquipmentBase GenerateItemOnPurchase(ItemRarity rarity, EquipmentData data)
    {
        // Generate a new instance of this item
        EquipmentBase item = new EquipmentBase();
        item.GenerateItemValues(data, rarity);
        return item;
    }
}
