using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StoryBeatType
{
    // === Generic Triggers ===
    DefaultDialogue,    // Can be played whenever but is actual character/plot stuff
    LowHealth,          // < 20%
    BarterSuccess,      // >=15 CHA
    BarterFail,         // < 10 CHA
    Repeatable,         // Not marked as seen, repeatable when you run out of other stuff to see
    // === Specific Plot Event Triggers ===
    NumRuns,
    EnemyKilled,
    KilledBy,
    DialogueCompleted,
    Item,
    // === For Looping ===
    enumSize
}

public enum DialoguePriority
{
    minRepeatable,  // Lowest priority; reserved for repeatable generic options ONLY
    p1,             // Low priority; for default, lowHP, barter, very low priority conversation/death stuff (like killing slimes)
    p2,             // Mid priority
    p3,             // High prioirty
    maxPriority     // Reserved ONLY for killing the Time Lich or certain Stellan interactions (BIG. PLOT. THINGS.)
}

public abstract class StoryBeat : ScriptableObject
{
    [Tooltip("Yarn node title base for the head node of responses to this beat (set automatically by generic, manually for specific)")]
    protected string yarnHeadNode = "";

    [Tooltip("Priority compared to other story beats for someone to comment ( > # = higher priority )")]
    [SerializeField] protected DialoguePriority priorityValue;

    [Tooltip("If this event can be commented on on future runs beyond the immediate next one")]
    [SerializeField] private bool carriesOver = false;

    [Tooltip("IF this story beat unlocks a journal content entry (or multiple), put that here; if not, leave it empty")]
    [SerializeField] private JournalContentID[] journalEntriesUnlocked;

    protected StoryBeatType beatType;     // Automatically set in the children

    [Tooltip("List of the speaking characters who have something to say about this event")]
    [SerializeField] private List<SpeakerID> speakersWithComments = new List<SpeakerID>();

    
    private Dictionary<StoryBeat,int> prereqStoryBeatDatabase = new Dictionary<StoryBeat,int>();
    
    [Tooltip("OPTIONAL list of prereq story beats that have to have number of completions >= int X")]
    [SerializeField] private List<StoryBeat> prereqBeats = new List<StoryBeat>();
    [Tooltip("List of numbers (of completion) associated with the above beats (bc dictionaries aren't serializable, we get the data this way and build it in start)")]
    [SerializeField] private List<int> prereqCompletionNumbers = new List<int>();

    public void CreatePrereqDatabase()
    {
        if( prereqBeats.Count != prereqCompletionNumbers.Count ){
            Debug.LogError("Story Beat " + yarnHeadNode + " number of prereq beats does not match the number of prereq completion numbers!");
            return;
        }
        // Build the prereq story beat database
        for(int i = 0; i < prereqBeats.Count; ++i){
            if(prereqStoryBeatDatabase.ContainsKey(prereqBeats[i])){
                continue;
            }
            prereqStoryBeatDatabase.Add( prereqBeats[i], prereqCompletionNumbers[i] );
        }
    }

    public abstract void SetValues();   // Set in the children

    public string GetYarnHeadNode()
    {
        return yarnHeadNode;
    }

    public DialoguePriority GetPriority()
    {
        return priorityValue;
    }

    public List<SpeakerID> GetSpeakersWithComments()
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

    public JournalContentID[] JournalEntriesUnlocked()
    {
        return journalEntriesUnlocked;
    }

    public StoryBeatType GetBeatType()
    {
        return beatType;
    }
}
