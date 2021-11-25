using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupHover : MonoBehaviour
{
    [HideInInspector] public List<TabButtonHover> tabButtons;

    // Color values for tabs
    public Color tabBackgroundIdle;
    public Color tabTitleIdle;
    public Color tabSubtitleIdle;
    public Color tabDescriptionIdle;

    public Color tabBackgroundHover;
    public Color tabTitleHover;
    public Color tabSubtitleHover;
    public Color tabDescriptionHover;

    public Color tabBackgroundActive;
    public Color tabTitleActive;
    public Color tabSubtitleActive;
    public Color tabDescriptionActive;

    private TabButtonHover selectedTab;
    private TabButtonHover highlightedTab;

    public void Subscribe(TabButtonHover button)
    {
        if(tabButtons == null){
            tabButtons = new List<TabButtonHover>();
        }        
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButtonHover button)
    {
        // If there already is a highlighted tab, un-highlight it
        if(highlightedTab != null){
            highlightedTab.OnExit();
        }

        // As long as this tab isn't the currently selected tab, highlight this one
        if( button != selectedTab ){
            highlightedTab = button;
            highlightedTab.OnHover();

            // Reset all the other tabs and set this tab to the hover appearance
            ResetTabs();
            button.background.color = tabBackgroundHover;
            button.tabTitle.color = tabTitleHover;
            button.tabSubtitle.color = tabSubtitleHover;
            button.tabDescription.color = tabDescriptionHover;
        }
    }

    public void OnTabExit(TabButtonHover button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButtonHover button)
    {
        // If you're clicking the currently selected tab again, deselect it and return
        if( selectedTab == button ){
            selectedTab.Deselect();
            selectedTab = null;
            OnTabEnter(button);
            return;
        }

        // If there already is a selected tab, deselect it
        if(selectedTab != null){
            selectedTab.Deselect();
        }

        // Select the new tab
        selectedTab = button;
        selectedTab.Select();

        // Reset all the other tabs and set this tab to the active appearance
        ResetTabs();
        button.background.color = tabBackgroundActive;
        button.tabTitle.color = tabTitleActive;
        button.tabSubtitle.color = tabSubtitleActive;
        button.tabDescription.color = tabDescriptionActive;
    }

    public void ResetTabs()
    {
        foreach(TabButtonHover button in tabButtons){
            if(selectedTab != null && button == selectedTab){
                continue;
            }
            button.background.color = tabBackgroundIdle;
            button.tabTitle.color = tabTitleIdle;
            button.tabSubtitle.color = tabSubtitleIdle;
            button.tabDescription.color = tabDescriptionIdle;
        }
    }

    public void UnselectAllTabs()
    {
        foreach(TabButtonHover button in tabButtons){
            button.Deselect();
            button.background.color = tabBackgroundIdle;
            button.tabTitle.color = tabTitleIdle;
            button.tabSubtitle.color = tabSubtitleIdle;
            button.tabDescription.color = tabDescriptionIdle;
        }
    }


}
