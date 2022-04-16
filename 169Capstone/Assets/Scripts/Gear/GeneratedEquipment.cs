using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - Used for both DROPPING items when enemies die, as well as generating SHOP items
    - Generates new item stats from certain requirements, given EquipmentData
*/
public class GeneratedEquipment : MonoBehaviour
{
    public StatType primaryLine = StatType.enumSize;
    public float primaryModifier = 0f;
    
    public float criticalChance = 0f;
    public float criticalDamage = 0f;
    public float attackSpeed = 0f;
    public int defense = 0;
    public float movementSpeed = 0f;
    public float stunChance = 0f;
    public float burnChance = 0f;
    public float slowChance = 0f;
    public float statusResist = 0f;
    public float dodgeChance = 0f;
    public int hp = 0;

    public EquipmentBaseData equipmentBaseData;

    public ItemRarity rarity;
    public int enhancementCount;

    public void SetAllEquipmentData(GeneratedEquipment _data)
    {
        primaryLine = _data.primaryLine;
        primaryModifier = _data.primaryModifier;
        
        criticalChance = _data.criticalChance;
        criticalDamage = _data.criticalDamage;
        attackSpeed = _data.attackSpeed;
        defense = _data.defense;
        movementSpeed = _data.movementSpeed;
        stunChance = _data.stunChance;
        burnChance = _data.burnChance;
        slowChance = _data.slowChance;
        statusResist = _data.statusResist;
        dodgeChance = _data.dodgeChance;
        hp = _data.hp;

        equipmentBaseData = _data.equipmentBaseData;

        rarity = _data.rarity;
        enhancementCount = _data.enhancementCount;
    }

    public void SetEquipmentBaseData(EquipmentBaseData _data, ItemRarity _rarity)
    {
        equipmentBaseData = _data;
        rarity = _rarity;
    }

    public void SetModifiers(StatType _primaryLine, float _primaryModifier, List<StatType> secondaryLines, int tier)
    {


        primaryLine = _primaryLine;
        primaryModifier = _primaryModifier;

        foreach (StatType line in secondaryLines){
            switch (line){
                case StatType.CritChance:
                    criticalChance += 0.02f;
                    continue;
                case StatType.CritDamage:
                    criticalDamage += 0.05f;
                    continue;
                case StatType.AttackSpeed:
                    attackSpeed += 0.02f;
                    continue;
                case StatType.Defense:
                    defense++;
                    continue;
                case StatType.MoveSpeed:
                    movementSpeed += 0.1f;
                    continue;
                case StatType.StunChance:
                    stunChance += 0.03f;       // TODO: How much should this be?
                    continue;
                case StatType.BurnChance:
                    burnChance += 0.03f;       // TODO: How much should this be?
                    continue;
                case StatType.SlowChance:
                    slowChance += 0.03f;       // TODO: How much should this be?
                    continue;
                case StatType.StatusResist:
                    statusResist += 0.02f;
                    continue;
                case StatType.DodgeChance:
                    dodgeChance += 0.02f;
                    continue;
                case StatType.HitPoints:
                    hp += (2 * tier);
                    continue;
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
        GameObject equippedItemObject = Instantiate( equipmentBaseData.EquippedItemPrefab(), PlayerInventory.instance.transform );

        // Get the Equipment (child) script from the prefab and set the spawned in data
        Equipment equippedItemData = equippedItemObject.GetComponent<Equipment>();
        equippedItemData.SetEquipmentData(this);

        // Get the inventory item slot
        InventoryItemSlot slot = equipmentBaseData.ItemSlot();

        // Now add the new item to the player's inventory (dictionary)
        PlayerInventory.instance.EquipItem( slot, equippedItemData );

        // If the item is a weapon, add JUST THE MODEL to the player model's hand
        if( slot == InventoryItemSlot.Weapon ){
            GameObject model = Instantiate( equipmentBaseData.EquippedWeaponModelPrefab(), Player.instance.handPos );
            PlayerInventory.instance.weaponModel = model;
            equippedItemData.InitializeModel(model);
        }

        // Destroy THIS game object because we no longer need it to store the generated item data
        Destroy(gameObject);
    }

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
