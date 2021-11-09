using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StoryBeatType
{
    creatureKilled,
    killedBy,
    dialogueCompleted,
    hasItem,
    enumSize
}

public enum DialoguePriority
{
    minPriority,    // Lowest priority, for default options
    p1,
    p2,
    p3,
    maxPriority     // Reserved for killing the Time Lich or certain Stellan interactions (BIG. PLOT. THINGS.)
}

public class StoryBeat : ScriptableObject
{
    [SerializeField] private string yarnHeadNode = "";        // Yarn node title (tag?) for the head node of responses to this beat (like a descriptive title)

    [SerializeField] private DialoguePriority priorityValue;  // Priority compared to other story beats for someone to comment ( > # = higher priority )

    [SerializeField] private bool carriesOver = false;        // If this event can be commented on on future runs beyond the immediate next one

    protected StoryBeatType beatType;     // Automatically set in the children

    [SerializeField] private HashSet<SpeakerData> speakersWithComments = new HashSet<SpeakerData>();    // List of the speaking characters who have something to say about this event

    private Dictionary<StoryBeat,int> prereqStoryBeatDatabase = new Dictionary<StoryBeat,int>();    // OPTIONAL list of prereq story beats that have to have numberOfCompletions >= int X
    [SerializeField] private List<StoryBeat> prereqBeats = new List<StoryBeat>();   // Because dictionaries aren't serializable, we get the data this way and build it in start
    [SerializeField] private List<int> prereqCompletionNumbers = new List<int>();
    // Switch to a list of key value pairs??? that doesn't seem to be working

    void Awake()
    {
        if( prereqBeats.Count != prereqCompletionNumbers.Count ){
            Debug.LogError("Story Beat " + yarnHeadNode + " number of prereq beats does not match the number of prereq completion numbers!");
            return;
        }
        // Build the prereq story beat database
        for(int i = 0; i < prereqBeats.Count; ++i){
            prereqStoryBeatDatabase.Add( prereqBeats[i], prereqCompletionNumbers[i] );
        }
    }

    public string GetYarnHeadNode()
    {
        return yarnHeadNode;
    }

    public DialoguePriority GetPriority()
    {
        return priorityValue;
    }

    public HashSet<SpeakerData> GetSpeakersWithComments()
    {
        return speakersWithComments;
    }

    public Dictionary<StoryBeat,int> GetPrereqStoryBeats()
    {
        return prereqStoryBeatDatabase;
    }

    public bool CarriesOver()
    {
        return carriesOver;
    }

    public StoryBeatType GetBeatType()
    {
        return beatType;
    }
}
