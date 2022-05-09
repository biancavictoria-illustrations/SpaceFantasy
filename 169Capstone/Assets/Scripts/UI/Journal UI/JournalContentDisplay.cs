using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalContentDisplay : MonoBehaviour
{
    // Each page has a UNIQUE ID (these need to be IN ORDER of the tab buttons)
    // TODO: Fill in the rest
    public enum JournalContentID{
        // CREW
        Atlan,
        Stellan,
        Bryn,
        Rhian,
        Doctor,
        Captain,

        // LOCATIONS
        Orbis,

        // STATS
        STR,
        DEX,
        INT,
        WIS,
        CON,
        CHA,

        // COMBAT
        Slime,
        Robert,
        BrutePest,
        //Harvester,
        TimeLich,

        // GEAR
        Electrum,
        StarShards,

        // For looping and stuff
        enumSize
    }

    public JournalSidebarTabGroup tabGroup;
    public int currentContentID;

    private JournalContentManager jcm;
    
    void Start()
    {
        // For convenience
        jcm = JournalContentManager.instance;

        ShowCurrentPanel();
    }

    private void ShowCurrentPanel()
    {
        // Show the content for the panel associated with JournalContentID panelIndex

        JournalContent content = jcm.contentDatabase[(JournalContentID)currentContentID];
    }

    public void SetPageIndex(int index)
    {
        currentContentID = index;
        ShowCurrentPanel();
    }
}
