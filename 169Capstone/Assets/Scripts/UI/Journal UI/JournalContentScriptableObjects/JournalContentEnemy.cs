using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JournalUI/JournalContentEnemy")]
public class JournalContentEnemy : JournalContent
{
    [Header("Enemy Data")]
    [SerializeField] private string threatLevel;
    [SerializeField] private string baseHealth;
    [SerializeField] private string baseDamage;
    
    [SerializeField] private string attackType;     // Ranged or melee
    [SerializeField] private string movementType;   // Flying or ground
    
    public string ThreatLevel()
    {
        return threatLevel;
    }

    public string BaseHealth()
    {
        return baseHealth;
    }

    public string BaseDamage()
    {
        return baseDamage;
    }

    public string MovementType()
    {
        return movementType;
    }

    public string AttackType()
    {
        return attackType;
    }
}
