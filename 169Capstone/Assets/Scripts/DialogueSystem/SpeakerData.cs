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
    public const string PORTRAIT_NEUTRAL = "neutral";
    public const string PORTRAIT_COMM = "comm";

    [Tooltip("The name that shows up to payers")]
    [SerializeField] private string speakerName;
    [Tooltip("Internal ID")]
    [SerializeField] private SpeakerID speakerID;
    
    [SerializeField] private YarnProgram yarnDialogue;

    [Tooltip("Times at which this NPC comments on how many runs you've done, in order")]
    [SerializeField] private List<int> hasNumRunDialogueList = new List<int>();

    [SerializeField] private Sprite portraitNeutral;
    [SerializeField] private Sprite portraitComm;

    [SerializeField] private bool isShopkeeper;

    public Sprite GetEmotionPortrait(string emotion)
    {
        switch(emotion)
        {
            case PORTRAIT_COMM:
                return portraitComm;
            default:
            case PORTRAIT_NEUTRAL:
                return portraitNeutral;
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

    public string GetYarnHeadNode()
    {
        return speakerID + "Start";
    }
}