using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalSidebarTabGroup : MonoBehaviour
{
    [HideInInspector] public List<JournalSidebarTabButton> tabButtons;

    // Color values for the backgrounds of tabs
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public JournalSidebarTabButton selectedTab;

    public JournalContentDisplay contentDisplay;

    public void Subscribe(JournalSidebarTabButton button)
    {
        if(tabButtons == null){
            tabButtons = new List<JournalSidebarTabButton>();
        }        

        tabButtons.Add(button);
    }

    void Start()
    {
        if(selectedTab){
            OnTabSelected(selectedTab);
        }
    }

    public void OnTabEnter(JournalSidebarTabButton button)
    {
        ResetTabs();
        if( selectedTab == null || button != selectedTab ){
            button.background.color = tabHover;
        }
    }

    public void OnTabExit(JournalSidebarTabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(JournalSidebarTabButton button)
    {
        // If there already is a selected tab, deselect it
        if(selectedTab != null){
            selectedTab.DeselectTabButton();
        }

        // Select the new tab
        selectedTab = button;
        selectedTab.SelectTabButton();

        // Reset all the other tabs and set this tab to the active appearance
        ResetTabs();
        button.background.color = tabActive;

        if(contentDisplay != null){
            // If it's unlocked, show the entry; otherwise, locked page
            contentDisplay.SetPageID(selectedTab.contentID, button.entryIsLocked);
        }
    }

    public void ResetTabs()
    {
        foreach(JournalSidebarTabButton button in tabButtons){
            if(selectedTab != null && button == selectedTab){
                continue;
            }
            button.background.color = tabIdle;
        }
    }

    public void SetTabUnlockedStatus()
    {
        Dictionary<JournalContentID, bool> journalUnlockStatusDatabase = GameManager.instance.journalContentManager.journalUnlockStatusDatabase;
        foreach( JournalSidebarTabButton journalEntryButton in tabButtons ){
            JournalContentID contentID = journalEntryButton.contentID;

            // If this ID isn't in the database, log a warning and continue
            if(!journalUnlockStatusDatabase.ContainsKey(contentID)){
                Debug.LogWarning("Content ID not found in the database: " + contentID);
                continue;
            }

            // If it's true, unlock it
            if( journalUnlockStatusDatabase[contentID] ){
                journalEntryButton.background.sprite = journalEntryButton.buttonSprite;
                journalEntryButton.entryIsLocked = false;
            }
            else{   // Else, lock it
                journalEntryButton.background.sprite = GameManager.instance.journalContentManager.JournalLockedSprite();
                journalEntryButton.entryIsLocked = true;
            }
        }
    }
}
