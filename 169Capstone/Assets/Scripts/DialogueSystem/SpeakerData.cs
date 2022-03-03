using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// EVERY speaker NPC has its own unique SpeakerID
// Start/head node for a speaker's yarn file is always their unique speakerID + "Start"
public enum SpeakerID
{
    Player,
    TimeLich,

    // === Hub World ===
    Stellan,

    // === On-Run Shopkeepers ===
    Bryn,
    Rhian,
    Doctor,
    Sorrel,

    enumSize
}

[CreateAssetMenu(menuName = "Narrative/Speaker")]
public class SpeakerData : ScriptableObject
{
    public const string EMOTION_NEUTRAL = "neutral";
    // Can add more of these (and more sprites) if we have more, like EMOTION_SURPRISE or EMOTION_ANGER

    [SerializeField] private string speakerName;      // The name that shows up to payers
    [SerializeField] private SpeakerID speakerID;     // Internal ID
    [SerializeField] private YarnProgram yarnDialogue;

    [SerializeField] private List<int> hasNumRunDialogueList = new List<int>();   // Times at which this NPC comments on how many runs you've done, in order

    [SerializeField] private Sprite portraitNeutral;
    // Can add different emotions

    [SerializeField] private bool isShopkeeper;

    public Sprite GetEmotionPortrait(string emotion)
    {
        switch(emotion)
        {
            default:
            case EMOTION_NEUTRAL: return portraitNeutral;
            // Can add different emotions
        }
    }

    public string SpeakerName()
    {
        return speakerName;
    }

    public SpeakerID SpeakerID()
    {
        return speakerID;
    }

    public YarnProgram YarnDialogue()
    {
        return yarnDialogue;
    }

    public List<int> NumRunDialogueList()
    {
        return hasNumRunDialogueList;
    }

    public bool IsShopkeeper()
    {
        return isShopkeeper;
    }
}
