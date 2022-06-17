using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalContentManager : MonoBehaviour
{
    public Dictionary<JournalContentID, JournalContent> contentDatabase {get; private set;}
    public Dictionary<JournalContentID, bool> journalUnlockStatusDatabase {get; private set;}

    [SerializeField] private Sprite journalLockedSprite;

    void Awake()
    {
        journalUnlockStatusDatabase = new Dictionary<JournalContentID, bool>();
        contentDatabase = new Dictionary<JournalContentID, JournalContent>();
        LoadAllJournalContentObjects();
    }

    private void LoadAllJournalContentObjects()
    {
        // Load in crew page data
        LoadContentFromLocation("JournalContent/Crew");

        // Load in location page data
        LoadContentFromLocation("JournalContent/Location");

        // Load in stat page data
        LoadContentFromLocation("JournalContent/Stats");

        // Load in enemy page data
        LoadContentFromLocation("JournalContent/Enemies");

        // Load in item page data
        LoadContentFromLocation("JournalContent/Items");
    }

    private void LoadContentFromLocation(string location)
    {
        Object[] journalContentList = Resources.LoadAll(location, typeof(JournalContent));
        foreach(Object c in journalContentList){
            JournalContent content = (JournalContent)c;
            if(!contentDatabase.ContainsKey(content.InternalID())){
                contentDatabase.Add(content.InternalID(), content);
            }
            journalUnlockStatusDatabase.Add(content.InternalID(), !content.LockedOnStart());
        }
    }

    public Sprite JournalLockedSprite()
    {
        return journalLockedSprite;
    }
    
    public void UnlockJournalEntry(JournalContentID[] contentIDs)
    {
        if(!PlayerInventory.hasCaptainsLog){
            return;
        }

        bool flag = false;
        foreach(JournalContentID id in contentIDs){
            if(!journalUnlockStatusDatabase.ContainsKey(id)){
                Debug.LogWarning("No content id key found in journalUnlockStatusDatabase for id: " + id);
                continue;
            }
            if(journalUnlockStatusDatabase[id]){
                continue;
            }

            // If any have not yet been set active, set flag to true so that we can enable the UI alert after
            flag = true;

            journalUnlockStatusDatabase[id] = true;
        }

        // If we're triggering journal alerts
        if(flag){
            // If there is an active NPC and that person is a shopkeeper, wait to trigger alert until shop closes
            if(NPC.ActiveNPC != null && NPC.ActiveNPC.SpeakerData().IsShopkeeper()){
                // If it's Stellan, tell his special UI to do the thing
                if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Stellan){
                    // Except if it's run 1 Stellan, in which case trigger it immediately cuz his shop isn't open yet (== 2 cuz run num updated on end of run)
                    if(GameManager.instance.currentRunNumber == 2){
                        TriggerJournalAlert();
                    }
                    else{
                        ShopUIStellan.journalAlertTriggeredOnShopClose = true;
                    }
                }
                // If it's not Stellan, use the generic shopUI
                else{
                    ShopUI.journalAlertTriggeredOnShopClose = true;
                }
            }
            // If this alert is NOT triggered by a shopkeeper, trigger the alert now
            else{
                TriggerJournalAlert();
            }
        }
    }

    private void TriggerJournalAlert()
    {
        AlertTextUI.instance.EnableJournalUpdatedAlert();
        StartCoroutine(AlertTextUI.instance.RemoveSecondaryAlertAfterSeconds());
    }

    public void SetJournalStatusOnLoad( Save.JournalContentStatus[] journalContentSaveStatus )
    {
        for(int i = 0; i < journalContentSaveStatus.Length; i++){
            journalUnlockStatusDatabase[ (JournalContentID)journalContentSaveStatus[i].contentID ] = journalContentSaveStatus[i].isUnlocked;
            // Debug.Log("Loading Journal Status w/ ID " + ((JournalContentID)i).ToString() + " isUnlocked: " + journalContentSaveStatus[i].isUnlocked);
        }
    }

    public void ReloadDefaultJournalStatus()
    {
        journalUnlockStatusDatabase.Clear();
        LoadAllJournalContentObjects();
    }
}
