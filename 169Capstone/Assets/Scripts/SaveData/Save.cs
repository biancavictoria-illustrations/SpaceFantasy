using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    #region Journal Stuff
        [System.Serializable] public struct JournalContentStatus{
            public int contentID;
            public bool isUnlocked;

            // Constructor
            public JournalContentStatus(int id, bool status){
                contentID = id;
                isUnlocked = status;
            }
        }

        public JournalContentStatus[] journalContentSaveStatus;
    #endregion

    #region Game Manager Stuff
        public int currentRunNumber;
        public bool hasKilledTimeLich;
        public bool killedTimeLichWithinFirstFourRuns;
        public int bossesKilled;
    #endregion

    // Inventory
    public int permanentCurrency;

    #region Permanent Upgrade Stuff
        public int totalPermanentCurrencySpent;

        public int startingHealthPotionQuantity;

        public int levelsInArmorPlating;
        public int levelsInExtensiveTraining;
        public int levelsInNatural20;
        public int levelsInPrecisionDrive;
        public int levelsInTimeLichKillerThing;

        public int strMin;
        public int strMax;
        public int dexMin;
        public int dexMax;
        public int intMin;
        public int intMax;
        public int wisMin;
        public int wisMax;
        public int conMin;
        public int conMax;
        public int charismaMin;
        public int charismaMax;
    #endregion

    #region Dialogue Stuff
        public bool talkedToStellan;

        public string[] visistedNodes;

        public int[] brynNumRunDialogueList;
        public bool brynListInitialized;
        public int[] stellanNumRunDialogueList;
        public bool stellanListInitialized;
        public int[] timeLichNumRunDialogueList;
        public bool lichListInitialized;
    #endregion

    #region Story Beat Status Values
        public StoryManager.BeatStatus[] storyBeatDatabaseStatuses;
        public StoryManager.BeatStatus[] itemStoryBeatStatuses;
        public StoryManager.BeatStatus[] genericStoryBeatStatuses;

        public string[] activeStoryBeatHeadNodes;
    #endregion

    // Constructor
    public Save(GameManager gameManager, PlayerInventory playerInventory, DialogueManager dialogueManager, StoryManager storyManager, PermanentUpgradeManager permanentUpgradeManager, JournalContentManager jcm)
    {
        // Game Manager Stuff
        currentRunNumber = gameManager.currentRunNumber;
        hasKilledTimeLich = gameManager.hasKilledTimeLich;
        killedTimeLichWithinFirstFourRuns = gameManager.killedTimeLichWithinFirstFourRuns;
        bossesKilled = gameManager.bossesKilled;

        // Inventory stuff
        permanentCurrency = playerInventory.permanentCurrency;

        // Permanent Upgrades (Skills & Stats)
        totalPermanentCurrencySpent = permanentUpgradeManager.totalPermanentCurrencySpent;
        
        startingHealthPotionQuantity = permanentUpgradeManager.startingHealthPotionQuantity;
        levelsInArmorPlating = permanentUpgradeManager.levelsInArmorPlating;
        levelsInExtensiveTraining = permanentUpgradeManager.levelsInExtensiveTraining;
        levelsInNatural20 = permanentUpgradeManager.levelsInNatural20;
        levelsInPrecisionDrive = permanentUpgradeManager.levelsInPrecisionDrive;
        levelsInTimeLichKillerThing = permanentUpgradeManager.levelsInTimeLichKillerThing;

        strMin = permanentUpgradeManager.strMin;
        strMax = permanentUpgradeManager.strMax;
        dexMin = permanentUpgradeManager.dexMin;
        dexMax = permanentUpgradeManager.dexMax;
        intMin = permanentUpgradeManager.intMin;
        intMax = permanentUpgradeManager.intMax;
        wisMin = permanentUpgradeManager.wisMin;
        wisMax = permanentUpgradeManager.wisMax;
        conMin = permanentUpgradeManager.conMin;
        conMax = permanentUpgradeManager.conMax;
        charismaMin = permanentUpgradeManager.charismaMin;
        charismaMax = permanentUpgradeManager.charismaMax;

        // Dialogue System Stuff
        talkedToStellan = storyManager.talkedToStellan;

        visistedNodes = new string[dialogueManager.visitedNodes.Count];
        SaveVisitedNodes(dialogueManager.visitedNodes);

        brynListInitialized = storyManager.brynListInitialized;
        brynNumRunDialogueList = new int[storyManager.brynNumRunDialogueList.Count];
        CharacterSpecificNumRunListToArray(SpeakerID.Bryn, storyManager.brynNumRunDialogueList);
        
        stellanListInitialized = storyManager.stellanListInitialized;
        stellanNumRunDialogueList = new int[storyManager.stellanNumRunDialogueList.Count];
        CharacterSpecificNumRunListToArray(SpeakerID.Stellan, storyManager.stellanNumRunDialogueList);
        
        lichListInitialized = storyManager.lichListInitialized;
        timeLichNumRunDialogueList = new int[storyManager.timeLichNumRunDialogueList.Count];
        CharacterSpecificNumRunListToArray(SpeakerID.TimeLich, storyManager.timeLichNumRunDialogueList);

        // Story Beat Status Values
        SaveStoryBeatStatuses(storyManager);

        activeStoryBeatHeadNodes = new string[storyManager.activeStoryBeats.Count];
        SaveActiveStoryBeats(storyManager.activeStoryBeats);

        // Journal status
        SaveJournalStatus(jcm.journalUnlockStatusDatabase);
    }

    private void SaveVisitedNodes( HashSet<string> _visisted )
    {
        int i = 0;
        foreach(string s in _visisted){
            visistedNodes[i] = s;
            i++;
        }
    }

    private void SaveStoryBeatStatuses( StoryManager sm )
    {
        storyBeatDatabaseStatuses = new StoryManager.BeatStatus[sm.storyBeatDatabase.Count];
        itemStoryBeatStatuses = new StoryManager.BeatStatus[sm.itemStoryBeats.Count];
        genericStoryBeatStatuses = new StoryManager.BeatStatus[sm.genericStoryBeats.Count];

        int i = 0;
        foreach( StoryManager.BeatStatus beatStatus in sm.storyBeatDatabase.Values ){
            storyBeatDatabaseStatuses[i] = beatStatus;
            i++;
        }

        i = 0;
        foreach( StoryManager.BeatStatus beatStatus in sm.itemStoryBeats.Values ){
            itemStoryBeatStatuses[i] = beatStatus;
            i++;
        }

        i = 0;
        foreach( StoryManager.BeatStatus beatStatus in sm.genericStoryBeats.Values ){
            genericStoryBeatStatuses[i] = beatStatus;
            i++;
        }
    }

    private void SaveActiveStoryBeats( HashSet<StoryBeat> activeBeats )
    {
        int i = 0;
        foreach(StoryBeat beat in activeBeats){
            activeStoryBeatHeadNodes[i] = beat.GetYarnHeadNode();
            i++;
        }
    }

    private void CharacterSpecificNumRunListToArray( SpeakerID speaker, List<int> numRunList )
    {
        switch(speaker){
            case SpeakerID.Bryn:
                for(int i = 0; i < numRunList.Count; i++){
                    brynNumRunDialogueList[i] = numRunList[i];
                }
                return;
            case SpeakerID.Stellan:
                for(int i = 0; i < numRunList.Count; i++){
                    stellanNumRunDialogueList[i] = numRunList[i];
                }
                return;
            case SpeakerID.TimeLich:
                for(int i = 0; i < numRunList.Count; i++){
                    timeLichNumRunDialogueList[i] = numRunList[i];
                }
                return;
        }
    }

    private void SaveJournalStatus( Dictionary<JournalContentID, bool> journalUnlockStatusDatabase )
    {
        journalContentSaveStatus = new JournalContentStatus[journalUnlockStatusDatabase.Count];

        int i = 0;
        foreach( KeyValuePair<JournalContentID, bool> contentStatus in journalUnlockStatusDatabase ){
            journalContentSaveStatus[i] = new JournalContentStatus((int)contentStatus.Key, contentStatus.Value);
            i++;
        }
    }
}