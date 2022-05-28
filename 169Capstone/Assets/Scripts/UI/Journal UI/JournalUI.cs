using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [SerializeField] private GameObject journalUIPanel;

    [SerializeField] private JournalSidebarTabGroup[] tabGroups;

    private GameObject devPanel;    // TEMP

    public void ToggleJournalActive(bool set)
    {
        journalUIPanel.SetActive(set);

        // TEMP
        if(!devPanel){
            devPanel = FindObjectOfType<DevPanel>()?.buttonPanel;
        }
        devPanel?.SetActive(!set);

        InGameUIManager.instance.SetGameUIActive(!set);
        InGameUIManager.instance.ToggleMiniMap(!set);

        if(!set){
            if(GameManager.instance.currentSceneName == GameManager.MAIN_HUB_STRING_NAME){
                InGameUIManager.instance.ToggleRunUI(false);
            }
        }
        else{
            foreach( JournalSidebarTabGroup tabGroup in tabGroups ){
                tabGroup.SetTabUnlockedStatus();
            }
        }
    }

    public void EnableJournalAlert()
    {
        AlertTextUI.instance.EnableJournalUpdatedAlert();
        StartCoroutine(AlertTextUI.instance.RemoveSecondaryAlertAfterSeconds());
    }    
}