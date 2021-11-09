using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Narrative/StoryBeat")]
public class StoryBeat : ScriptableObject
{    
    [SerializeField] private string yarnHeadNode = "";      // Yarn node title (tag?) for the head node of responses to this beat (like a descriptive title)

    // Event Triggers
    [SerializeField] private StoryBeatType storyBeatType;   // The event that occured that triggers this beat
    [SerializeField] private CreatureID otherCreature;      // Creature associated with this event trigger (creature killed or killed by, OR NPC talked to)
    [SerializeField] private string otherDialogue = "";     // OPTIONAL if triggered by a dialogueCompleted event, the yarn head node for the dialogue (branch?) completed

    [SerializeField] private bool carriesOver = false;      // If this event can be commented on on future runs beyond the immediate next one
    [SerializeField] private int priorityValue = 0;         // Priority compared to other story beats for someone to comment ( > # = higher priority )

    [SerializeField] private List<SpeakerData> speakersWithComments = new List<SpeakerData>();    // List of the speaking characters who have something to say about this event

    // TODO: How do you set this up ahhhhhh
    [SerializeField] private Dictionary<StoryBeat,int> prereqStoryBeats = new Dictionary<StoryBeat,int>();    // OPTIONAL list of prereq story beats that have to have numberOfCompletions >= int X


    public string GetYarnHeadNode()
    {
        return yarnHeadNode;
    }

    public StoryBeatType GetBeatType()
    {
        return storyBeatType;
    }

    public CreatureID GetOtherCreature()
    {
        return otherCreature;
    }

    public string GetOtherDialogue()
    {
        return otherDialogue;
    }

    public bool CarriesOver()
    {
        return carriesOver;
    }

    public int GetPriorityValue()
    {
        return priorityValue;
    }

    public List<SpeakerData> GetSpeakersWithComments()
    {
        return speakersWithComments;
    }

    public Dictionary<StoryBeat,int> GetPrereqStoryBeats()
    {
        return prereqStoryBeats;
    }
}