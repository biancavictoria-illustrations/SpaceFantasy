using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - Used for both DROPPING items when enemies die, as well as generating SHOP items
    - Generates new item stats from certain requirements, given EquipmentData
*/
public struct SpawnedEquipmentData
{
    public ItemLine primaryLine;
    public float primaryModifier;
    public float criticalChance;
    public float criticalDamage;
    public float attackSpeed;
    public int defense;
    public float movementSpeed;
    public float effectChance;
    public float effectResist;
    public float dodgeChance;
    public int hp;

    public EquipmentBaseData equipmentBaseData;

    public ItemRarity rarity;
    public int enhancementCount;
}

public class GeneratedEquipment : MonoBehaviour
{
    public SpawnedEquipmentData data;

    void Start()
    {
        // Set default values
        data.primaryLine = ItemLine.enumSize;
        data.primaryModifier = 0f;
        data.criticalChance = 0f;
        data.criticalChance = 0f;
        data.criticalDamage = 0f;
        data.attackSpeed = 0f;
        data.defense = 0;
        data.movementSpeed = 0f;
        data.effectChance = 0f;
        data.effectResist = 0f;
        data.dodgeChance = 0f;
        data.hp = 0;
    }

    public void SetAllEquipmentData(SpawnedEquipmentData _data)
    {
        data = _data;
    }

    public void SetEquipmentBaseData(EquipmentBaseData _data, ItemRarity _rarity)
    {
        data.equipmentBaseData = _data;
        data.rarity = _rarity;
    }

    public void SetModifiers(ItemLine _primaryLine, float _primaryModifier, List<ItemLine> secondaryLines, int tier)
    {
        data.primaryLine = _primaryLine;
        data.primaryModifier = _primaryModifier;

        foreach (ItemLine line in secondaryLines)
        {
            switch (line)
            {
                case ItemLine.CritChance:
                    data.criticalChance += 0.02f;
                    break;
                case ItemLine.CritDamage:
                    data.criticalDamage += 0.05f;
                    break;
                case ItemLine.AttackSpeed:
                    data.attackSpeed += 0.02f;
                    break;
                case ItemLine.Defense:
                    data.defense++;
                    break;
                case ItemLine.MovementSpeed:
                    data.movementSpeed += 0.1f;
                    break;
                case ItemLine.EffectChance:
                    data.effectChance += 0.03f;
                    break;
                case ItemLine.EffectResist:
                    data.effectResist += 0.02f;
                    break;
                case ItemLine.DodgeChance:
                    data.dodgeChance += 0.02f;
                    break;
                case ItemLine.HPIncrease:
                    data.hp += (2 * tier);
                    break;
            }
        }
    }

    public void AddEnhancements()
    {
        // TODO
    }

    public void EquipGeneratedItem()
    {
        // Instantiate the newly equipped item from the prefab in the base data (as a child of the PlayerInventory)
        GameObject equippedItemObject = Instantiate( data.equipmentBaseData.EquippedItemPrefab(), PlayerInventory.instance.transform );

        // Get the Equipment (child) script from the prefab and set the spawned in data
        Equipment equippedItemData = equippedItemObject.GetComponent<Equipment>();
        equippedItemData.SetEquipmentData(data);

        // Get the inventory item slot
        InventoryItemSlot slot = data.equipmentBaseData.ItemSlot();

        // Now add the new item to the player's inventory (dictionary)
        PlayerInventory.instance.EquipItem( slot, equippedItemData );

        // If the item is a weapon, add JUST THE MODEL to the player model's hand
        if( slot == InventoryItemSlot.Weapon ){
            GameObject model = Instantiate( data.equipmentBaseData.ItemModelPrefab(), Player.instance.handPos );
            PlayerInventory.instance.weaponModel = model;
        }

        // Destroy THIS game object because we no longer need it to store the generated item data
        Destroy(gameObject);
    }
}
