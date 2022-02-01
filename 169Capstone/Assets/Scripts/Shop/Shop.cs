using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventory shopKeeper;
    private List<string> inventoryList;

    public HashSet<EquipmentBase> inventory {get; private set;}


    void Start()
    {
        GenerateShopInventory();
    }

    private void GenerateShopInventory()
    {
        int tier = ObjectManager.bossesKilled;

        inventoryList = new List<string>();
        inventory = new HashSet<EquipmentBase>();

        inventoryList.Add(shopKeeper.Slot1());
        inventoryList.Add(shopKeeper.Slot2());
        inventoryList.Add(shopKeeper.Slot3());
        inventoryList.Add(shopKeeper.Slot4());
        inventoryList.Add(shopKeeper.Slot5());

        for(int i = 0; i < inventoryList.Count; i++)
        {
            if(inventoryList[i].Contains("[tier x]"))
            {
                // inventoryList[i] = inventoryList[i].Replace("[tier x]", rarity[tier]);

                // Generate a new item with rarity = tier #, then add it to the inventory HashSet
                inventory.Add( GenerateItem((ItemRarity)tier) );
            }
            else if(inventoryList[i].Contains("[tier x+1]"))
            {
                // inventoryList[i] = inventoryList[i].Replace("[tier x+1]", rarity[tier + 1]);

                inventory.Add( GenerateItem((ItemRarity)(tier+1)) );
            }
        }
    }

    private EquipmentBase GenerateItem(ItemRarity rarity)
    {
        // TODO: Randomly selects 5 items from the ShopKeeperInventory pool (no repeats, presumably?)
        // temp: just picks the first one
        EquipmentData data = shopKeeper.Items()[0];

        // Generate a new instance of this item
        EquipmentBase item = new EquipmentBase();
        item.GenerateItemValues(data, rarity);
        return item;
    }
}
