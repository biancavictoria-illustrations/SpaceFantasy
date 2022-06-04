using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventory shopKeeper;
    private List<RarityAssignmentTier> inventoryList;
    private HashSet<int> indexes;

    [SerializeField] private LineTable shopLineTable;
    [SerializeField] private GameObject shopItemPrefab;

    [HideInInspector] public bool shopJustRestocked = false;

    public List<GeneratedEquipment> inventory {get; private set;}

    void Start()
    {
        inventory = new List<GeneratedEquipment>();

        // Add a listener so that we regenerate shop inventory each time we gear tier up
        GameManager.instance.OnTierIncreased.AddListener(GenerateShopInventory);
        // Then generate starting tier 0 inventory
        GenerateShopInventory( GameManager.instance.gearTier );
    }

    public void ClearShopInventory()
    {
        // TODO: Destroy all the actual game objects
        // for(int i = 0; i < inventory.Count; i++){
        //     Destroy(inventory[i].gameObject);
        // }

        // Clear the list
        inventory.Clear();
    }

    public void GenerateShopInventory(int tier)
    {
        // If this is NOT the first time we've generated the inventory, clear it and start again
        if( inventory.Count > 0 ){
            ClearShopInventory();
        }

        indexes = new HashSet<int>();

        inventoryList = new List<RarityAssignmentTier>();

        inventoryList.Add(shopKeeper.Slot1());
        inventoryList.Add(shopKeeper.Slot2());
        inventoryList.Add(shopKeeper.Slot3());
        inventoryList.Add(shopKeeper.Slot4());
        inventoryList.Add(shopKeeper.Slot5());

        // Most of the shop items generate as the CURRENT gear tier # value
        int tier1 = tier;
        // Fewer generate as that value + 1 (so Uncommon if gear tier = Common, etc.)
        int tier2 = tier + 1;

        // If Legendary, there's nothing above so make the less frequent version one tier down (Epic)
        if( (ItemRarity)tier1 == ItemRarity.Legendary ){
            tier2 = (int)ItemRarity.Epic;
        }

        for(int i = 0; i < inventoryList.Count; i++){
            if(inventoryList[i] == RarityAssignmentTier.X){
                // Generate a new item with rarity = tier #, then add it to the inventory HashSet
                inventory.Add( GenerateItem((ItemRarity)tier1) );
            }
            else if(inventoryList[i] == RarityAssignmentTier.X_1){
                inventory.Add( GenerateItem((ItemRarity)tier2) );
            }
        }

        shopJustRestocked = true;
    }

    private GeneratedEquipment GenerateItem(ItemRarity rarity)
    {
        // Generate a new (randomly selected) item with the determined rarity
        GameObject itemObject = Instantiate(shopItemPrefab);
        GeneratedEquipment generatedEquipment = itemObject.GetComponent<GeneratedEquipment>();

        // Use the static item generator to generate the lines and stuff from here
        GenericItemGenerator.GenerateItem(ref generatedEquipment, PickItemBaseDataFromPool(), rarity, shopLineTable);

        return generatedEquipment;
    }

    private EquipmentBaseData PickItemBaseDataFromPool()
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
