using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventory shopKeeper;
    public List<string> inventory;

    void Start()
    {
        Generate();
    }

    private void Generate()
    {
        int tier = ObjectManager.bossesKilled;
        inventory = new List<string>();
        string[] rarity = { "Common", "Uncommon", "Rare", "Epic", "Legendary" };

        inventory.Add(shopKeeper.slot1);
        inventory.Add(shopKeeper.slot2);
        inventory.Add(shopKeeper.slot3);
        inventory.Add(shopKeeper.slot4);
        inventory.Add(shopKeeper.slot5);

        for(int i = 0; i < inventory.Count; i++)
        {
            if(inventory[i].Contains("[tier x]"))
            {
                inventory[i] = inventory[i].Replace("[tier x]", rarity[tier]);
            }
            else if(inventory[i].Contains("[tier x+1]"))
            {
                inventory[i] = inventory[i].Replace("[tier x+1]", rarity[tier + 1]);
            }
        }
    }
}
