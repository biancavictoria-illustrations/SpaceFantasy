using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{
    public static NPC ActiveNPC {get; private set;}

    [SerializeField] private SpeakerData speakerData;

    public GameObject newDialogueAlert;

    public bool haveTalkedToThisRun {get; private set;}

    [Tooltip("ONLY true for Time Lich (and set at runtime for first Stellan interaction)")]
    private bool forceNextDialogueOnTriggerEnter = false;
    private float timeToWaitForAutoDialogue = 0f;

    void Start()
    {
        if(speakerData == null){
            Debug.LogWarning("No speaker data set for NPC! NPC not able to initialize properly.");
            return;
        }

        InitializeAssociatedNumRunDialogueList();

        if(speakerData.SpeakerID() == SpeakerID.TimeLich || (speakerData.SpeakerID() == SpeakerID.Stellan && GameManager.instance.currentRunNumber == 2)){
            forceNextDialogueOnTriggerEnter = true;
            if(speakerData.SpeakerID() == SpeakerID.Stellan){
                timeToWaitForAutoDialogue = GameManager.DEFAULT_AUTO_DIALOGUE_WAIT_TIME;
            }
        }

        SetHaveTalkedToNPCThisRun(HaveTalkedToNPC());
    }

    // When player gets within range, they can start dialogue
    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveNPC = this;

            // If they're marked to autoplay dialogue this time, immediately start dialogue on trigger enter
            if(forceNextDialogueOnTriggerEnter && !haveTalkedToThisRun){
                forceNextDialogueOnTriggerEnter = false;
                StartCoroutine(GameManager.instance.AutoRunDialogueAfterTime(timeToWaitForAutoDialogue));
                return;
            }

            // If you haven't talked to them yet, enable interact alert
            if(!haveTalkedToThisRun){
                AlertTextUI.instance.EnableInteractAlert();
            }
            // If you have talked to them already but they're a shopkeeper, enable the shop alert
            else if(speakerData.IsShopkeeper()){
                AlertTextUI.instance.EnableShopAlert();
            }
        }
    }

    // When player leaves the range, they can no longer start the dialogue
    private void OnTriggerExit(Collider other)
    {
        // If the player is no longer near this NPC
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveNPC = null;
            AlertTextUI.instance.DisableAlert();
        }
    }

    public void SetHaveTalkedToNPCThisRun(bool set)
    {
        haveTalkedToThisRun = set;

        // Check if it exists cuz presumably time lich won't have one so we don't need to set it
        if(newDialogueAlert && !forceNextDialogueOnTriggerEnter){
            newDialogueAlert.SetActive(!set);
        }
    }

    // When you finish dialogue, call this to deactivate the NPC
    public void TalkedToNPC()
    {
        SetHaveTalkedToNPCThisRun(true);

        SpeakerID speaker = speakerData.SpeakerID();
        if( speaker == SpeakerID.Bryn ){
            StoryManager.instance.talkedToBryn = true;
            return;
        }
        else if( speaker == SpeakerID.Rhian ){
            StoryManager.instance.talkedToRhian = true;
            return;
        }
        else if( speaker == SpeakerID.Sorrel ){
            StoryManager.instance.talkedToSorrel = true;
            return;
        }
        else if( speaker == SpeakerID.Doctor ){
            StoryManager.instance.talkedToDoctor = true;
            return;
        }
        else if( speaker == SpeakerID.Stellan ){
            StoryManager.instance.talkedToStellan = true;
            return;
        }
        else if( speaker == SpeakerID.TimeLich ){
            StoryManager.instance.talkedToLich = true;
            return;
        }
        Debug.LogError("Tried to log talking to an NPC who does not exist! SpeakerID: " + speaker);
    }

    // Ask the story manager if we've talked to this NPC yet
    private bool HaveTalkedToNPC()
    {
        SpeakerID speaker = speakerData.SpeakerID();
        if( speaker == SpeakerID.Bryn ){
            return StoryManager.instance.talkedToBryn;
        }
        else if( speaker == SpeakerID.Rhian ){
            return StoryManager.instance.talkedToRhian;
        }
        else if( speaker == SpeakerID.Sorrel ){
            return StoryManager.instance.talkedToSorrel;
        }
        else if( speaker == SpeakerID.Doctor ){
            return StoryManager.instance.talkedToDoctor;
        }
        else if( speaker == SpeakerID.Stellan ){
            return StoryManager.instance.talkedToStellan;
        }
        else if( speaker == SpeakerID.TimeLich ){
            return StoryManager.instance.talkedToLich;
        }
        Debug.LogError("Tried to check if you have talked to an NPC who does not exist! SpeakerID: " + speaker);
        return false;
    }

    private void InitializeAssociatedNumRunDialogueList()
    {
        StoryManager storyManager = StoryManager.instance;

        switch( speakerData.SpeakerID() ){
            case SpeakerID.Bryn:
                if(!storyManager.brynListInitialized){
                    storyManager.brynListInitialized = true;
                    storyManager.brynNumRunDialogueList = new List<int>(speakerData.NumRunDialogueList());
                }                
                return;
            case SpeakerID.TimeLich:
                if(!storyManager.lichListInitialized){
                    storyManager.lichListInitialized = true;
                    storyManager.timeLichNumRunDialogueList = new List<int>(speakerData.NumRunDialogueList());
                }     
                return;
            case SpeakerID.Doctor:
                if(!storyManager.doctorListInitialized){
                    storyManager.doctorListInitialized = true;
                    storyManager.doctorNumRunDialogueList = new List<int>(speakerData.NumRunDialogueList());
                }     
                return;
            case SpeakerID.Stellan:
                if(!storyManager.stellanListInitialized){
                    storyManager.stellanListInitialized = true;
                    storyManager.stellanNumRunDialogueList = new List<int>(speakerData.NumRunDialogueList());
                }     
                return;
            case SpeakerID.Rhian:
                if(!storyManager.rhianListInitialized){
                    storyManager.rhianListInitialized = true;
                    storyManager.rhianNumRunDialogueList = new List<int>(speakerData.NumRunDialogueList());
                }     
                return;
        }
    }

    public List<int> GetNumRunDialogueList()
    {
        switch( speakerData.SpeakerID() ){
            case SpeakerID.Bryn:              
                return StoryManager.instance.brynNumRunDialogueList;
            case SpeakerID.TimeLich:   
                return StoryManager.instance.timeLichNumRunDialogueList;
            case SpeakerID.Doctor:   
                return StoryManager.instance.doctorNumRunDialogueList;
            case SpeakerID.Stellan: 
                return StoryManager.instance.stellanNumRunDialogueList;
            case SpeakerID.Rhian: 
                return StoryManager.instance.rhianNumRunDialogueList;
        }
        Debug.LogError("No num run dialogue list found for SpeakerID: " + speakerData.SpeakerID());
        return null;
    }

    public SpeakerData SpeakerData()
    {
        return speakerData;
    }
}
