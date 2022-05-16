using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JournalUI/JournalContentItem")]
public class JournalContentItem : JournalContent
{
    [SerializeField] private string origin;

    [Tooltip("What the currency is /used for/ OR what the item /does/, depending on currency or gear")]
    [SerializeField] private string purpose;

    [Tooltip("GEAR ONLY")]
    [SerializeField] private string gearMechanicalDescription;
}
