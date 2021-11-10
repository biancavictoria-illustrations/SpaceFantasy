using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Every item (gear, weapons) has its own unique ID
public enum ItemID
{
    sword,
    helm,
    enumSize
}

[CreateAssetMenu(menuName = "Narrative/StoryBeatItem")]
public class StoryBeatItem : StoryBeat
{    
    // Unique Trigger
    [SerializeField] private ItemID item;     // The item that can trigger dialogue

    void Awake()
    {
        beatType = StoryBeatType.hasItem;
    }

    public ItemID GetItem()
    {
        return item;
    }
}