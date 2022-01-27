using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public string itemName = "";
    public string itemType;
    private string primaryLine;
    private float primaryModifier;
    private float criticalChance = 0f;
    private float criticalDamage = 0f;
    private float attackSpeed = 0f;
    private int defense = 0;
    private float movementSpeed = 0f;
    private float effectChance = 0f;
    private float effectResist = 0f;
    private float dodgeChance = 0f;
    private int hp = 0;

    public void SetModifiers(string _primaryLine, float _primaryModifier, List<string> secondaryLines, int tier)
    {
        primaryLine = _primaryLine;
        primaryModifier = _primaryModifier;

        foreach (string line in secondaryLines)
        {
            switch (line)
            {
                case "Critical Chance":
                    criticalChance += 0.02f;
                    break;
                case "Critical Damage":
                    criticalDamage += 0.05f;
                    break;
                case "Attack Speed":
                    attackSpeed += 0.02f;
                    break;
                case "Defense":
                    defense++;
                    break;
                case "Movement Speed":
                    movementSpeed += 0.1f;
                    break;
                case "Effect Chance":
                    effectChance += 0.03f;
                    break;
                case "Effect Resist":
                    effectResist += 0.02f;
                    break;
                case "Dodge Chance":
                    dodgeChance += 0.02f;
                    break;
                case "HP Increase":
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

        switch(itemName)
        {
            case "":
                Debug.LogError("Cannot drop item: item does not have an item name");
                break;
            case "Berserker's Zweihander":
                mesh.sharedMesh = Resources.Load<Mesh>("Longsword");
                break;
            case "Bow And Arrows":
                mesh.sharedMesh = Resources.Load<Mesh>("Bow");
                break;
            default:
                mesh.sharedMesh = Resources.Load<Mesh>(itemName);
                break;
        }

        renderer.enabled = true;
    }

    public void PickupItem()
    {
        // when player chooses to select this item, the actual item has to be instantiated with modifiers included
    }
}
