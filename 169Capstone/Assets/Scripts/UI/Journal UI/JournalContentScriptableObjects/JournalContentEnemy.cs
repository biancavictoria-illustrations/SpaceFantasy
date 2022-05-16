using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JournalUI/JournalContentEnemy")]
public class JournalContentEnemy : JournalContent
{
    [SerializeField] private string threatLevel;
    [SerializeField] private string attackPatterns; // Some sort of "skills" type category with the types of attacks they do (maybe?)
    [SerializeField] private string baseHealth;
    [SerializeField] private string baseDamage;
    [SerializeField] private string movementType;   // Flying or ground
    [SerializeField] private string attackType;     // Ranged or melee

    
}
