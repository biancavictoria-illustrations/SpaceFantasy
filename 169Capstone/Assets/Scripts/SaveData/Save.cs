using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    // Game Manager Stuff
    public int currentRunNumber;
    public bool hasKilledTimeLich;

    // Inventory
    public int permanentCurrency;

    // Permanent Upgrade Stuff
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

    // Dialogue Stuff
    public string[] visistedNodes;

    public int[] brynNumRunDialogueList;
    public bool brynListInitialized;
    public int[] stellanNumRunDialogueList;
    public bool stellanListInitialized;
    public int[] timeLichNumRunDialogueList;
    public bool lichListInitialized;
    public int[] doctorNumRunDialogueList;
    public bool doctorListInitialized;

    // Story Beat Status Values
    // TODO

    // Constructor
    public Save(GameManager gameManager, PlayerInventory playerInventory, DialogueManager dialogueManager, StoryManager storyManager, PermanentUpgradeManager permanentUpgradeManager)
    {
        // Game Manager Stuff
        currentRunNumber = gameManager.currentRunNumber;
        hasKilledTimeLich = gameManager.hasKilledTimeLich;

        // Inventory stuff
        permanentCurrency = playerInventory.permanentCurrency;

        // Permanent upgrades (skills and stats)
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
        visistedNodes = new string[dialogueManager.visitedNodes.Count];
        SaveVisitedNodes(dialogueManager.visitedNodes);

        brynListInitialized = storyManager.brynListInitialized;
        brynNumRunDialogueList = new int[storyManager.brynNumRunDialogueList.Count];
        CharacterSpecificNumRunListToArray(SpeakerID.Bryn, storyManager.brynNumRunDialogueList);
        
        stellanListInitialized = storyManager.stellanListInitialized;
        stellanNumRunDialogueList = new int[storyManager.stellanNumRunDialogueList.Count];
        CharacterSpecificNumRunListToArray(SpeakerID.Stellan, storyManager.stellanNumRunDialogueList);
        
        doctorListInitialized = storyManager.doctorListInitialized;
        doctorNumRunDialogueList = new int[storyManager.doctorNumRunDialogueList.Count];
        CharacterSpecificNumRunListToArray(SpeakerID.Doctor, storyManager.doctorNumRunDialogueList);
        
        lichListInitialized = storyManager.lichListInitialized;
        timeLichNumRunDialogueList = new int[storyManager.timeLichNumRunDialogueList.Count];
        CharacterSpecificNumRunListToArray(SpeakerID.TimeLich, storyManager.timeLichNumRunDialogueList);

        // Story Beat Status Values
        // TODO
    }

    private void SaveVisitedNodes( HashSet<string> _visisted )
    {
        int i = 0;
        foreach(string s in _visisted){
            visistedNodes[i] = s;
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
            case SpeakerID.Doctor:
                for(int i = 0; i < numRunList.Count; i++){
                    doctorNumRunDialogueList[i] = numRunList[i];
                }
                return;
            case SpeakerID.TimeLich:
                for(int i = 0; i < numRunList.Count; i++){
                    timeLichNumRunDialogueList[i] = numRunList[i];
                }
                return;
        }
    }
}