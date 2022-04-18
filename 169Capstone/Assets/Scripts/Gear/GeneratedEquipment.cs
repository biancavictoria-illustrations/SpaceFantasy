using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - Used for both DROPPING items when enemies die, as well as generating SHOP items
    - Generates new item stats from certain requirements, given EquipmentData
*/
public class GeneratedEquipment : MonoBehaviour
{
    public float primaryLineValue = 0f;
    
    public float criticalChance = 0f;
    public float criticalDamage = 0f;
    public float attackSpeed = 0f;
    public int defense = 0;
    public float movementSpeed = 0f;
    public float statusEffectChance = 0f;
    public float statusResist = 0f;
    public float dodgeChance = 0f;
    public int hp = 0;

    public EquipmentBaseData equipmentBaseData;

    public ItemRarity rarity;
    public int enhancementCount;

    public void SetAllEquipmentData(GeneratedEquipment _data)
    {
        primaryLineValue = _data.primaryLineValue;

        criticalChance = _data.criticalChance;
        criticalDamage = _data.criticalDamage;
        attackSpeed = _data.attackSpeed;
        defense = _data.defense;
        movementSpeed = _data.movementSpeed;
        statusEffectChance = _data.statusEffectChance;
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

    public void SetAllLineModifiers(List<StatType> secondaryLines, int _enhancementCount)
    {
        enhancementCount = _enhancementCount;

        SetPrimaryLineValueByRarityAndType();

        foreach (StatType line in secondaryLines){
            UpgradeStatValue(line);
        }

        if(enhancementCount > 0 && secondaryLines.Count > 0){
            AddEnhancements(secondaryLines, _enhancementCount);
        }
    }

    private void AddEnhancements(List<StatType> secondaryLines, int _count)
    {
        while(_count > 0){
            // Randomly pick one of the lines to upgrade
            int index = Random.Range(0, secondaryLines.Count);
            UpgradeStatValue( secondaryLines[index] );
            _count--;
        }
    }

    private void UpgradeStatValue(StatType type)
    {
        switch (type){
            case StatType.CritChance:
                criticalChance += 0.02f;
                break;
            case StatType.CritDamage:
                criticalDamage += 0.05f;
                break;
            case StatType.AttackSpeed:
                attackSpeed += 0.02f;
                break;
            case StatType.Defense:
                defense++;
                break;
            case StatType.MoveSpeed:
                movementSpeed += 0.1f;
                break;
            case StatType.StatusEffectChance:
                statusEffectChance += 0.03f;
                break;
            case StatType.StatusResist:
                statusResist += 0.02f;
                break;
            case StatType.DodgeChance:
                dodgeChance += 0.02f;
                break;
            case StatType.HitPoints:
                hp += (2 * (int)rarity);
                break;
        }
    }

    private void SetPrimaryLineValueByRarityAndType()
    {
        // If a common weapon, don't give it a primary line
        if(equipmentBaseData.ItemSlot() == InventoryItemSlot.Weapon && rarity == ItemRarity.Common){
            return;
        }

        InventoryItemSlot itemSlot = equipmentBaseData.ItemSlot();
        
        int rarityMultiplier = (int)rarity + 1; // Adds 1 to rarity so that it can start at Common (tier 0)

        if( itemSlot == InventoryItemSlot.Weapon ){
            primaryLineValue = 2f * (int)rarity; // +2 Damage Increase; does NOT add 1 to the rarity multipler cuz starts at Uncommon level instead
        }
        else if( itemSlot == InventoryItemSlot.Helmet ){
            primaryLineValue = 0.1f * rarityMultiplier; // 10% Health Increase
        }
        else if( itemSlot == InventoryItemSlot.Accessory ){
            primaryLineValue = 0.04f * rarityMultiplier; // 4% Effect Chance Increase
        }
        else if( itemSlot == InventoryItemSlot.Legs ){
            primaryLineValue = 0.05f * rarityMultiplier; // 5% Move Speed Increase
        }
        else{
            Debug.LogError("Cannot set primary line value for item type: " + itemSlot);
        }
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

    public float GetSecondaryLineValueFromStatType(StatType type)
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
            case StatType.StatusEffectChance:
                return statusEffectChance;
            case StatType.StatusResist:
                return statusResist;
            case StatType.DodgeChance:
                return dodgeChance;
            case StatType.HitPoints:
                return hp;
        }
        Debug.LogError("No value found for stat type: " + type + "; invalid secondary line type.");
        return -1;
    }

    #region UI Stuff

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
            case StatType.StatusEffectChance:
                return "Status Effect Chance";
            case StatType.DodgeChance:
                return "Dodge Chance";
            case StatType.HitPoints:
                return "Hit Points";        // TODO: What do we want to call this???
            case StatType.STRDamage:
                return "STR Damage";
            case StatType.DEXDamage:
                return "DEX Damage";
            case StatType.INTDamage:
                return "INT Damage";
            case StatType.WISDamage:
                return "WIS Damage";
        }
        Debug.LogError("No stat string found for stat type: " + type);
        return "ERROR";
    }

    #endregion
}
