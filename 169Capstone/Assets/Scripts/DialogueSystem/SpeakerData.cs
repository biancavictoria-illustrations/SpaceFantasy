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
    public const string PORTRAIT_COMM_SAD = "commSad";
    
    public const string PORTRAIT_ANGRY = "angry";
    public const string PORTRAIT_SAD = "sad";
    public const string PORTRAIT_SHOCKED = "shocked";
    public const string PORTRAIT_CONFUSED = "confused";
    public const string PORTRAIT_CRINGE = "cringe";
    public const string PORTRAIT_ANNOYED = "annoyed";

    [Tooltip("The name that shows up to payers")]
    [SerializeField] private string speakerName;
    [Tooltip("Internal ID")]
    [SerializeField] private SpeakerID speakerID;
    [SerializeField] private bool isShopkeeper;
    
    [SerializeField] private YarnProgram yarnDialogue;

    [Tooltip("Times at which this NPC comments on how many runs you've done, in order")]
    [SerializeField] private List<int> hasNumRunDialogueList = new List<int>();

    [Header("--- PORTRAITS ---")]
    [SerializeField] private Sprite portraitNeutral;

    [Tooltip("Hologram/comm (for Stellan)")]
    [SerializeField] private Sprite portraitComm;
    [Tooltip("(For Stellan)")]
    [SerializeField] private Sprite portraitCommSad;

    [SerializeField] private Sprite portraitAngry;
    [SerializeField] private Sprite portraitSad;
    [SerializeField] private Sprite portraitShocked;
    [SerializeField] private Sprite portraitConfused;
    [SerializeField] private Sprite portraitCringe;
    [SerializeField] private Sprite portraitAnnoyed;

    public Sprite GetEmotionPortrait(string portraitCode)
    {
        switch(portraitCode)
        {
            case PORTRAIT_COMM:
                return portraitComm;
            case PORTRAIT_COMM_SAD:
                return portraitCommSad;

            case PORTRAIT_ANGRY:
                return portraitAngry;

            case PORTRAIT_SAD:
                return portraitSad;

            case PORTRAIT_SHOCKED:
                return portraitShocked;

            case PORTRAIT_CONFUSED:
                return portraitConfused;
            
            case PORTRAIT_CRINGE:
                return portraitCringe;
            
            case PORTRAIT_ANNOYED:
                return portraitAnnoyed;

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