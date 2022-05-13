using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalContentDisplay : MonoBehaviour
{
    // Each page has a UNIQUE ID (these need to be IN ORDER of the tab buttons)
    // TODO: Fill in the rest
    public enum JournalContentID{
        // CREW
        Stellan,
        Bryn,
        Rhian,
        Doctor,
        Captain,
        Orby,
        Atlan,  // Hide him, unlock him later maybe?

        // LOCATIONS
        TheOrbis,

        // STATS
        STR,
        DEX,
        CON,
        INT,
        WIS,
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

    public enum ContentType{
        Crew,
        Locations,
        Stats,
        Combat,
        Items
    }

    public JournalSidebarTabGroup tabGroup;
    [HideInInspector] public int currentContentID;
    [SerializeField] private JournalContentManager jcm;

    [SerializeField] private ContentType contentType;

    private const string ID_PREFIX = "<b>ID:</b> ";

    [Header("Top Header Panel")]
    [SerializeField] private TMP_Text contentSectionTitle;
    [SerializeField] private TMP_Text contentSectionSubTitleLeft;
    [SerializeField] private TMP_Text contentSectionSubTitleRight;

    [Header("Body Panel")]
    [SerializeField] private TMP_Text bodyContent1;
    [SerializeField] private TMP_Text bodyContent2;
    [SerializeField] private TMP_Text bodyContent3;
    [SerializeField] private TMP_Text bodyContent4;
    [SerializeField] private TMP_Text bodyContent5;
    [SerializeField] private TMP_Text bodyContent6;
    [SerializeField] private TMP_Text mainBodyContent;

    [Header("Right Panel")]
    [SerializeField] private Image contentImage;
    
    void Start()
    {
        ShowCurrentContentPage();
    }

    private void ShowCurrentContentPage()
    {
        // Show the content for the panel associated with JournalContentID panelIndex
        JournalContent content = jcm.contentDatabase[(JournalContentID)currentContentID];

        switch( contentType ){
            case ContentType.Crew:
                SetCrewValues((JournalContentCrew)content);
                break;
            case ContentType.Locations:
                // SetCrewValues((JournalContentCrew)content);
                break;
            case ContentType.Stats:
                // SetCrewValues((JournalContentCrew)content);
                break;
            case ContentType.Combat:
                // SetCrewValues((JournalContentCrew)content);
                break;
            case ContentType.Items:
                // SetCrewValues((JournalContentCrew)content);
                break;
        }
    }

    #region Set Values By Type
        private void SetCrewValues(JournalContentCrew content)
        {
            // Top Header Panel
            contentSectionTitle.text = content.EntryName();
            contentSectionSubTitleLeft.text = content.JobTitle();
            contentSectionSubTitleRight.text = ID_PREFIX + content.EntryID();

            // Body Panel
            bodyContent1.text = "<b>DoB:</b> " + content.Birthday();
            bodyContent2.text = "<b>HOME PLANET:</b> " + content.PlaceOfBirth();
            bodyContent3.text = "<b>RACE:</b> " + content.Race();
            bodyContent4.text = "<b>HEIGHT:</b> " + content.Height();
            bodyContent5.text = "<b>STRENGTHS:</b> " + content.Strengths();
            bodyContent6.text = "<b>WEAKNESSES:</b> " + content.Weaknesses();
            mainBodyContent.text = "<b>PERFORMANCE REVIEW:</b>\n" + content.ReportNotes();

            // Right Panel
            // contentImage.sprite = content.ProfilePicture();  // TODO: Uncomment once we have these
        }

        // TODO: the other types
    #endregion

    public void SetPageIndex(int index)
    {
        currentContentID = index;
        ShowCurrentContentPage();
    }
}
