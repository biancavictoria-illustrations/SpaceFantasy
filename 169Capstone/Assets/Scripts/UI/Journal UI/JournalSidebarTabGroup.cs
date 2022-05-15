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
            contentDisplay.SetPageID(selectedTab.contentID);
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
}
