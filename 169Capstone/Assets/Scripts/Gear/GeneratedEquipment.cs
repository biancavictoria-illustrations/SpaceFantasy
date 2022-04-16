using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - Used for both DROPPING items when enemies die, as well as generating SHOP items
    - Generates new item stats from certain requirements, given EquipmentData
*/
public struct SpawnedEquipmentData
{
    public StatType primaryLine;
    public float primaryModifier;
    
    public float criticalChance;
    public float criticalDamage;
    public float attackSpeed;
    public int defense;
    public float movementSpeed;
    public float stunChance;
    public float burnChance;
    public float slowChance;
    public float statusResist;
    public float dodgeChance;
    public int hp;

    public EquipmentBaseData equipmentBaseData;

    public ItemRarity rarity;
    public int enhancementCount;

    public float GetValueFromStatType(StatType type)
    {
        switch(type){
            case StatType.CritChance:
                return criticalChance;
            case StatType.CritDamage:
                return criticalDamage;
            case StatType.AttackSpeed:
                return attackSpeed;
            case StatType.Defense:
                return defense;
            case StatType.MoveSpeed:
                return movementSpeed;
            case StatType.StunChance:
                return stunChance;
            case StatType.BurnChance:
                return burnChance;
            case StatType.SlowChance:
                return slowChance;
            case StatType.StatusResist:
                return statusResist;
            case StatType.DodgeChance:
                return dodgeChance;
            case StatType.HitPoints:
                return hp;
        }
        Debug.LogError("No value found for stat type: " + type);
        return -1;
    }

    public string GetPlayerFacingStatName(StatType type)
    {
        switch(type){
            case StatType.CritChance:
                return "Crit Chance";
            case StatType.CritDamage:
                return "Crit Damage";
            case StatType.AttackSpeed:
                return "Attack Speed";
            case StatType.Defense:
                return "Defense";
            case StatType.MoveSpeed:
                return "Movement Speed";
            case StatType.StunChance:
                return "Stun Chance";
            case StatType.BurnChance:
                return "Burn Chance";
            case StatType.SlowChance:
                return "Slow Chance";
            case StatType.StatusResist:
                return "Status Resist Chance";
            case StatType.DodgeChance:
                return "Dodge Chance";
            case StatType.HitPoints:
                return "Hit Points";        // TODO: What do we want to call this???
        }
        Debug.LogError("No string found for stat type: " + type);
        return "ERROR";
    }
}

public class GeneratedEquipment : MonoBehaviour
{
    public SpawnedEquipmentData data;

    void Start()
    {
        // Set default values
        data.primaryLine = StatType.enumSize;
        data.primaryModifier = 0f;
        data.criticalChance = 0f;
        data.criticalChance = 0f;
        data.criticalDamage = 0f;
        data.attackSpeed = 0f;
        data.defense = 0;
        data.movementSpeed = 0f;
        data.stunChance = 0f;
        data.burnChance = 0f;
        data.slowChance = 0f;
        data.statusResist = 0f;
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

    public void SetModifiers(StatType _primaryLine, float _primaryModifier, List<StatType> secondaryLines, int tier)
    {
        data.primaryLine = _primaryLine;
        data.primaryModifier = _primaryModifier;

        foreach (StatType line in secondaryLines)
        {
            switch (line)
            {
                case StatType.CritChance:
                    data.criticalChance += 0.02f;
                    break;
                case StatType.CritDamage:
                    data.criticalDamage += 0.05f;
                    break;
                case StatType.AttackSpeed:
                    data.attackSpeed += 0.02f;
                    break;
                case StatType.Defense:
                    data.defense++;
                    break;
                case StatType.MoveSpeed:
                    data.movementSpeed += 0.1f;
                    break;
                case StatType.StunChance:
                    data.stunChance += 0.03f;       // TODO: How much should this be?
                    break;
                case StatType.BurnChance:
                    data.burnChance += 0.03f;       // TODO: How much should this be?
                    break;
                case StatType.SlowChance:
                    data.slowChance += 0.03f;       // TODO: How much should this be?
                    break;
                case StatType.StatusResist:
                    data.statusResist += 0.02f;
                    break;
                case StatType.DodgeChance:
                    data.dodgeChance += 0.02f;
                    break;
                case StatType.HitPoints:
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
            GameObject model = Instantiate( data.equipmentBaseData.EquippedWeaponModelPrefab(), Player.instance.handPos );
            PlayerInventory.instance.weaponModel = model;
            equippedItemData.InitializeModel(model);
        }

        // Destroy THIS game object because we no longer need it to store the generated item data
        Destroy(gameObject);
    }
}
