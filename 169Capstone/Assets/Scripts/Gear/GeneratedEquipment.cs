using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    - Used for both DROPPING items when enemies die, as well as generating SHOP items
    - Generates new item stats from certain requirements, given EquipmentData
    - Is not yet the ACTUAL item object -> that is instantiated if the player equips/purchases the item
*/

public class GeneratedEquipment : MonoBehaviour
{
    public EquipmentData equipmentData {get; private set;}

    public ItemLine primaryLine {get; private set;}
    public float primaryModifier {get; private set;}

    public float criticalChance {get; private set;}
    public float criticalDamage {get; private set;}
    public float attackSpeed {get; private set;}
    public int defense {get; private set;}
    public float movementSpeed {get; private set;}
    public float effectChance {get; private set;}
    public float effectResist {get; private set;}
    public float dodgeChance {get; private set;}
    public int hp {get; private set;}


    void Start()
    {
        // Set default values
        criticalChance = 0f;
        criticalDamage = 0f;
        attackSpeed = 0f;
        defense = 0;
        movementSpeed = 0f;
        effectChance = 0f;
        effectResist = 0f;
        dodgeChance = 0f;
        hp = 0;
    }

    public void SetEquipmentData(EquipmentData data)
    {
        equipmentData = data;
    }

    public void SetModifiers(ItemLine _primaryLine, float _primaryModifier, List<ItemLine> secondaryLines, int tier)
    {
        primaryLine = _primaryLine;
        primaryModifier = _primaryModifier;

        foreach (ItemLine line in secondaryLines)
        {
            switch (line)
            {
                case ItemLine.CritChance:
                    criticalChance += 0.02f;
                    break;
                case ItemLine.CritDamage:
                    criticalDamage += 0.05f;
                    break;
                case ItemLine.AttackSpeed:
                    attackSpeed += 0.02f;
                    break;
                case ItemLine.Defense:
                    defense++;
                    break;
                case ItemLine.MovementSpeed:
                    movementSpeed += 0.1f;
                    break;
                case ItemLine.EffectChance:
                    effectChance += 0.03f;
                    break;
                case ItemLine.EffectResist:
                    effectResist += 0.02f;
                    break;
                case ItemLine.DodgeChance:
                    dodgeChance += 0.02f;
                    break;
                case ItemLine.HPIncrease:
                    hp += (2 * tier);
                    break;
            }
        }
    }

    public void AddEnhancements()
    {
        // Later
    }

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
        // TODO: when player chooses to select this item, the actual item has to be instantiated with modifiers included
    }
}
