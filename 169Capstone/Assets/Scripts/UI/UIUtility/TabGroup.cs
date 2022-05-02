using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [HideInInspector] public List<TabButton> tabButtons;

    // Color values for the backgrounds of tabs
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public TabButton selectedTab;

    // public List<GameObject> objectsToSwap;

    public PanelGroup panelGroup;

    public void Subscribe(TabButton button)
    {
        if(tabButtons == null){
            tabButtons = new List<TabButton>();
        }        

        tabButtons.Add(button);
    }

    void Start()
    {
        if(selectedTab){
            OnTabSelected(selectedTab);
        }
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if( selectedTab == null || button != selectedTab ){
            button.background.color = tabHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
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

        // Swap the UI visible
        // int index = button.transform.GetSiblingIndex();
        // for(int i = 0; i < objectsToSwap.Count; i++){
        //     if(i == index){
        //         objectsToSwap[i].SetActive(true);
        //     }
        //     else{
        //         objectsToSwap[i].SetActive(false);
        //     }
        // }

        // This might work in place of the above??? maybe comment that out
        if(panelGroup != null){
            panelGroup.SetPageIndex(button.transform.GetSiblingIndex());
        }
    }

    public void ResetTabs()
    {
        foreach(TabButton button in tabButtons){
            if(selectedTab != null && button == selectedTab){
                continue;
            }
            button.background.color = tabIdle;
        }
    }
}
