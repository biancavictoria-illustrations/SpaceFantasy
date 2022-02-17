using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

/*
    - Used for both DROPPING items when enemies die, as well as generating SHOP items
    - Generates new item stats from certain requirements, given EquipmentData
    - Parent class of all types of items, including weapons
*/
public class GeneratedEquipment : MonoBehaviour
{
    public EquipmentData equipmentData {get; private set;}

    // Generated when the item is created
    public ItemRarity rarity {get; private set;}
    public int enhancementCount {get; private set;}
    public EquipmentStats stats;

    public int currentCost {get; private set;}  // For shop items

    // Dropped gear you are in range of
    public static GeneratedEquipment ActiveGearDrop {get; private set;}

    private const float rarityMultiplierBase = 1.2f;
    private const float costPowerValue = 1.25f;
    private const float timeFactor = 0.0606f;

    void Start()
    {
        CalculateCurrentCost();

        // Set default values
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
        float cost = equipmentData.BaseCost();      // Set base cost

        // Set the rarity multiplier (rarity multiplier base to a power of the ItemRarity value)
        float rarityMultiplier = Mathf.Pow(rarityMultiplierBase, (int)rarity);

        // Set coeff to (time factor * time in min) * stage factor
        int playerFactor = 1;
        float timeInMin = 0;        // TODO: Set to time in min
        float stageFactor = 1f;     // TODO: Set to stage factor
        float coeff = (playerFactor + timeInMin * timeFactor) * stageFactor;

        // Raise coeff to the power of the costPowerValue
        coeff = Mathf.Pow(coeff,costPowerValue);

        cost = cost * coeff * rarityMultiplier;     // Multiply base cost by coeff and rarity multiplier
        currentCost = (int)Mathf.Floor(cost);       // Get int using Floor to round
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
        // TODO
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
                mesh.sharedMesh = Resources.Load<Mesh>("Items/Longsword");
                break;
            case ItemID.BowAndArrows:
                mesh.sharedMesh = Resources.Load<Mesh>("Items/Bow");
                break;
            default:
                mesh.sharedMesh = Resources.Load<Mesh>("Items/" + equipmentData.ItemID().ToString());
                break;
        }

        renderer.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveGearDrop = this;
            AlertTextUI.instance.EnableItemPickupAlert();

            Debug.Log("ITEM ACTIVE");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveGearDrop = null;
            AlertTextUI.instance.DisableAlert();

            Debug.Log("ITEM INACTIVE (left radius)");
        }
    }
}
