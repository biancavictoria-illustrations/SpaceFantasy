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
        SecondaryStats,

        // ENEMIES
        Slime,
        Robert,
        BrutePest,
        //Harvester,
        TimeLich,

        // ITEMS
        Electrum,
        StarShards,

        // For looping and stuff
        enumSize
    }

    public enum ContentType{
        Crew,
        Locations,
        Stats,
        Enemy,
        Items
    }

    public JournalSidebarTabGroup tabGroup;
    [SerializeField] private JournalContentManager jcm;

    [SerializeField] private ContentType contentType;

    [Tooltip("Set to default first/top page value")]
    public JournalContentID activePageID;

    private const string ID_PREFIX = "<b>ID:</b> ";
    private const string RESEARCH_PREFIX = "<b>RESEARCH NOTES:</b>\n";

    #region UI Elements
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

        [Header("Stat Panel Options")]
        [SerializeField] private GameObject coreStatPanel;
        [SerializeField] private GameObject secondaryStatPanel;
    #endregion
    
    void Start()
    {
        ShowCurrentContentPage();
    }

    private void ShowCurrentContentPage()
    {
        Debug.Log("Setting journal values for: " + activePageID);

        // Show the content for the panel associated with JournalContentID panelIndex
        JournalContent content = jcm.contentDatabase[activePageID];

        switch( contentType ){
            case ContentType.Crew:
                SetCrewValues((JournalContentCrew)content);
                break;
            case ContentType.Locations:
                // TODO
                // SetCrewValues((JournalContentCrew)content);
                break;
            case ContentType.Stats:
                SetStatValues((JournalContentStat)content);
                break;
            case ContentType.Enemy:
                SetEnemyValues((JournalContentEnemy)content);
                break;
            case ContentType.Items:
                SetItemValues((JournalContentItem)content);
                break;
        }
    }

    #region Set Values By Type
        private void SetDefaultValues(JournalContent content)
        {
            // Top Header Panel
            contentSectionTitle.text = content.EntryName();
            contentSectionSubTitleRight.text = ID_PREFIX + content.EntryID();

            // Right Panel
            if(content.ProfilePicture()){
                contentImage.sprite = content.ProfilePicture();
            }
        }

        private void SetCrewValues(JournalContentCrew content)
        {
            SetDefaultValues(content);

            // Top Header Panel
            contentSectionSubTitleLeft.text = content.JobTitle();
            // contentSectionSubTitleRight.text = ID_PREFIX + content.EntryID();

            // Body Panel
            bodyContent1.text = "<b>DoB:</b> " + content.Birthday();
            bodyContent2.text = "<b>HOME PLANET:</b> " + content.PlaceOfBirth();
            bodyContent3.text = "<b>RACE:</b> " + content.Race();
            bodyContent4.text = "<b>HEIGHT:</b> " + content.Height();
            bodyContent5.text = "<b>STRENGTHS:</b> " + content.Strengths();
            bodyContent6.text = "<b>WEAKNESSES:</b> " + content.Weaknesses();
            mainBodyContent.text = "<b>PERFORMANCE REVIEW:</b>\n" + content.ReportNotes();
        }

        private void SetStatValues(JournalContentStat content)
        {
            bool isSecondaryStatsEntry = content.InternalID() == JournalContentID.SecondaryStats;
            secondaryStatPanel.SetActive(isSecondaryStatsEntry);
            coreStatPanel.SetActive(!isSecondaryStatsEntry);

            if(isSecondaryStatsEntry){
                // panel is made all in the editor, nothing populates at runtime
                return;
            }

            SetDefaultValues(content);

            // Top Header Panel

            // Body Panel
            bodyContent1.text = "<b>SECONDARY STAT:</b> " + content.AssociatedSecondaryStats();
            mainBodyContent.text = RESEARCH_PREFIX + content.ReportNotes();
        }

        private void SetEnemyValues(JournalContentEnemy content)
        {
            SetDefaultValues(content);

            // Top Header Panel

            // Body Panel
            mainBodyContent.text = RESEARCH_PREFIX + content.ReportNotes();
        }

        private void SetItemValues(JournalContentItem content)
        {
            SetDefaultValues(content);

            // Top Header Panel

            // Body Panel
            mainBodyContent.text = RESEARCH_PREFIX + content.ReportNotes();
        }

        // TODO: the last type
    #endregion

    public void SetPageID(JournalContentID id)
    {
        activePageID = id;
        ShowCurrentContentPage();
    }
}
