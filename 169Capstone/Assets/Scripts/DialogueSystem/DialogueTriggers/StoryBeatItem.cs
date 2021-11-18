using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Every item (gear, weapons) has its own unique ID
public enum ItemID
{
    Sword,
    Helm,
    enumSize
}

[CreateAssetMenu(menuName = "Narrative/StoryBeatItem")]
public class StoryBeatItem : StoryBeat
{    
    // Unique Trigger
    [SerializeField] private ItemID item;     // The item that can trigger dialogue

    public override void SetValues()
    {
        beatType = StoryBeatType.Item;

        yarnHeadNode = beatType.ToString() + item;
    }

    public ItemID GetItem()
    {
        return item;
    }
}