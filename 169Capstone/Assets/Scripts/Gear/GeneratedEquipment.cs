using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - Used for both DROPPING items when enemies die, as well as generating SHOP items
    - Generates new item stats from certain requirements, given EquipmentData
    - Is not yet the ACTUAL item object -> that is instantiated if the player equips/purchases the item
*/

public struct EquipmentStats
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
}

public class GeneratedEquipment : MonoBehaviour
{
    public EquipmentData equipmentData {get; private set;}
    public ItemRarity rarity {get; private set;}
    public EquipmentStats stats;
    public int currentCost {get; private set;}

    void Start()
    {
        // Set default values
        currentCost = equipmentData.BaseCost();
        stats.primaryLine = ItemLine.enumSize;
        stats.primaryModifier = 0f;
        stats.criticalChance = 0f;
        stats.criticalChance = 0f;
        stats.criticalDamage = 0f;
        stats.attackSpeed = 0f;
        stats.defense = 0;
        stats.movementSpeed = 0f;
        stats.effectChance = 0f;
        stats.effectResist = 0f;
        stats.dodgeChance = 0f;
        stats.hp = 0;
    }

    public void CalculateCurrentCost()
    {
        // TODO
        currentCost = equipmentData.BaseCost();
    }

    public void SetEquipmentData(EquipmentData _data, ItemRarity _rarity)
    {
        equipmentData = _data;
        rarity = _rarity;
    }

    public void SetModifiers(ItemLine _primaryLine, float _primaryModifier, List<ItemLine> secondaryLines, int tier)
    {
        stats.primaryLine = _primaryLine;
        stats.primaryModifier = _primaryModifier;

        foreach (ItemLine line in secondaryLines)
        {
            switch (line)
            {
                case ItemLine.CritChance:
                    stats.criticalChance += 0.02f;
                    break;
                case ItemLine.CritDamage:
                    stats.criticalDamage += 0.05f;
                    break;
                case ItemLine.AttackSpeed:
                    stats.attackSpeed += 0.02f;
                    break;
                case ItemLine.Defense:
                    stats.defense++;
                    break;
                case ItemLine.MovementSpeed:
                    stats.movementSpeed += 0.1f;
                    break;
                case ItemLine.EffectChance:
                    stats.effectChance += 0.03f;
                    break;
                case ItemLine.EffectResist:
                    stats.effectResist += 0.02f;
                    break;
                case ItemLine.DodgeChance:
                    stats.dodgeChance += 0.02f;
                    break;
                case ItemLine.HPIncrease:
                    stats.hp += (2 * tier);
                    break;
            }
        }
    }

    public void AddEnhancements()
    {
        // Later
    }

    // Called in the Drop script
    public void Drop()
    {
        //transform.position = pos;
        MeshFilter mesh = gameObject.GetComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();

        switch(equipmentData.ItemID())
        {
            case ItemID.enumSize:
                Debug.LogError("Cannot drop item: item does not have an ItemID");
                break;
            case ItemID.BerserkersZweihander:
                mesh.sharedMesh = Resources.Load<Mesh>("Longsword");
                break;
            case ItemID.BowAndArrows:
                mesh.sharedMesh = Resources.Load<Mesh>("Bow");
                break;
            default:
                mesh.sharedMesh = Resources.Load<Mesh>(equipmentData.ItemID().ToString());
                break;
        }

        renderer.enabled = true;
    }

    public void EquipItem()
    {
        // TODO: When player chooses to equip this item, instantiate the actual item
        
    }
}
