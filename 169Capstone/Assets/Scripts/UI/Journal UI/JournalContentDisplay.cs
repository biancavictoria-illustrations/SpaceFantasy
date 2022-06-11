using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalContentDisplay : MonoBehaviour
{
    public enum ContentType{
        Crew,
        Locations,
        Stats,
        Enemy,
        Items
    }

    public JournalSidebarTabGroup tabGroup;
    private JournalContentManager jcm;

    [SerializeField] private ContentType contentType;
    [HideInInspector] public JournalContentID activePageID;

    private const string ID_PREFIX = "<b>ID:</b> ";
    private const string RESEARCH_PREFIX = "<b>RESEARCH NOTES:</b>\n";

    #region UI Elements
        [SerializeField] private GameObject mainContentPanel;
        [SerializeField] private GameObject lockedPanel;

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

        [Header("Crew Panel Options")]
        [SerializeField] private GameObject captainPanel;

        [Header("Stat Panel Options")]
        [SerializeField] private GameObject secondaryStatPanel;

        [Header("Location Panel Options")]
        [SerializeField] private GameObject galaxyPanel;
    #endregion
    
    void Start()
    {
        jcm = GameManager.instance.journalContentManager;

        switch(contentType){
            case ContentType.Crew:
                activePageID = JournalContentID.Captain;
                break;
            case ContentType.Locations:
                activePageID = JournalContentID.TheOrbis;
                break;
            case ContentType.Stats:
                activePageID = JournalContentID.STR;
                break;
            case ContentType.Enemy:
                activePageID = JournalContentID.SlimeDefault;
                break;
            case ContentType.Items:
                activePageID = JournalContentID.Electrum;
                break;
        }
        ShowCurrentContentPage();
    }

    private void ShowCurrentContentPage()
    {
        if(!jcm){
            jcm = GameManager.instance.journalContentManager;
        }
        
        // Show the content for the panel associated with JournalContentID panelIndex
        JournalContent content = jcm.contentDatabase[activePageID];

        switch( contentType ){
            case ContentType.Crew:
                SetCrewValues((JournalContentCrew)content);
                break;
            case ContentType.Locations:
                SetLocationValues((JournalContentLocation)content);
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
            if(contentImage){
                if(content.ProfilePicture())
                    contentImage.sprite = content.ProfilePicture();
                else
                    contentImage.sprite = null;
            }
        }

        private void SetCrewValues(JournalContentCrew content)
        {
            bool isCaptainPanel = content.InternalID() == JournalContentID.Captain;
            captainPanel.SetActive(isCaptainPanel);
            mainContentPanel.SetActive(!isCaptainPanel);

            if(isCaptainPanel){
                return;
            }

            SetDefaultValues(content);

            // Top Header Panel
            contentSectionSubTitleLeft.text = content.JobTitle();

            // Body Panel
            bodyContent1.text = "<b>AGE:</b> " + content.Age();
            bodyContent2.text = "<b>HOME PLANET:</b> " + content.PlaceOfBirth();
            bodyContent3.text = "<b>RACE:</b> " + content.Race();
            bodyContent4.text = "<b>HEIGHT:</b> " + content.Height();
            bodyContent5.text = "<b>STRENGTHS:</b> " + content.Strengths();
            bodyContent6.text = "<b>WEAKNESSES:</b> " + content.Weaknesses();

            if(content.InternalID() != JournalContentID.Atlan){
                mainBodyContent.text = "<b>PERFORMANCE REVIEW:</b>\n" + content.ReportNotes();
            }
            else{
                mainBodyContent.text = RESEARCH_PREFIX + content.ReportNotes();
            }
        }

        private void SetStatValues(JournalContentStat content)
        {
            bool isSecondaryStatsEntry = content.InternalID() == JournalContentID.SecondaryStats;
            secondaryStatPanel.SetActive(isSecondaryStatsEntry);
            mainContentPanel.SetActive(!isSecondaryStatsEntry);

            if(isSecondaryStatsEntry){
                // panel is made all in the editor, nothing populates at runtime
                return;
            }

            SetDefaultValues(content);

            // Body Panel
            bodyContent1.text = "<b>SECONDARY STAT:</b> " + content.AssociatedSecondaryStats();
            mainBodyContent.text = RESEARCH_PREFIX + content.ReportNotes();
        }

        private void SetEnemyValues(JournalContentEnemy content)
        {
            SetDefaultValues(content);

            // Top Header Panel
            contentSectionSubTitleLeft.text = "<b>THREAT LEVEL: </b>" + content.ThreatLevel();

            // Body Panel
            bodyContent1.text = "<b>BASE DAMAGE:</b> " + content.BaseDamage();
            bodyContent2.text = "<b>BASE HEALTH:</b> " + content.BaseHealth();
            bodyContent3.text = "<b>ATTACK TYPE:</b> " + content.AttackType();
            bodyContent4.text = "<b>MOVEMENT TYPE:</b> " + content.MovementType();
            mainBodyContent.text = RESEARCH_PREFIX + content.ReportNotes();
        }

        private void SetItemValues(JournalContentItem content)
        {
            SetDefaultValues(content);

            // Top Header Panel
            contentSectionSubTitleLeft.text = content.ItemType();

            // Body Panel
            bodyContent1.text = "<b>CAPABILITIES:</b>\n" + content.MechanicalDescription();
            mainBodyContent.text = RESEARCH_PREFIX + content.ReportNotes();
        }

        private void SetLocationValues(JournalContentLocation content)
        {
            if(galaxyPanel){
                bool isGalaxyPanel = content.InternalID() == JournalContentID.Galaxy;
                galaxyPanel.SetActive(isGalaxyPanel);
                mainContentPanel.SetActive(!isGalaxyPanel);

                if(isGalaxyPanel){
                    // panel is made all in the editor, nothing populates at runtime
                    return;
                }
            }
            
            SetDefaultValues(content);

            // Top Header Panel (if a left subheader)

            // Body Panel
            
            // TODO: generate random date to put there
            bodyContent1.text = "<b>DATE OF LAST INSPECTION:</b> " + "???";
            
            mainBodyContent.text = "<b>MAINTENANCE REVIEW:</b>\n" + content.ReportNotes();
        }
    #endregion

    public void SetPageID(JournalContentID id, bool isLocked)
    {
        activePageID = id;

        lockedPanel.SetActive(isLocked);

        if(!isLocked){     // If it's unlocked, activate the main content panel and find the right data to display
            mainContentPanel.SetActive(true);
            ShowCurrentContentPage();
        }
        else{   // If it's locked, set all the content panel options inactive
            mainContentPanel.SetActive(false);
            if(secondaryStatPanel){
                secondaryStatPanel.SetActive(false);
            }
            if(galaxyPanel){
                galaxyPanel.SetActive(false);
            }
            if(captainPanel){
                captainPanel.SetActive(false);
            }
        }
    }
}
