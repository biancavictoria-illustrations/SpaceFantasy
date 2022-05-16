using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "JournalUI/JournalContentItem")]
public class JournalContentItem : JournalContent
{
    [Header("Item Data")]
    [SerializeField] private string itemType;

    // Put ORIGIN + PURPOSE in research notes instead

    [TextArea(15,20)]
    [SerializeField] private string mechanicalDescription;

    public string ItemType()
    {
        return itemType;
    }

    public string MechanicalDescription()
    {
        return mechanicalDescription;
    }
}
