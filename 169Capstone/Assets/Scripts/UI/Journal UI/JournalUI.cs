using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [SerializeField] private GameObject journalUIPanel;

    [SerializeField] private JournalSidebarTabGroup[] tabGroups;

    [SerializeField] private Selectable topButton;

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

        if(GameManager.instance.InSceneWithRandomGeneration())
            InGameUIManager.instance.ToggleMiniMap(!set);

        if(set){
            AudioManager.Instance.PlaySFX(AudioManager.SFX.CaptainsLogOpen);
            topButton.Select();
        }
        else{
            AudioManager.Instance.PlaySFX(AudioManager.SFX.CaptainsLogClose);
        }

        if(!set && GameManager.instance.currentSceneName == GameManager.MAIN_HUB_STRING_NAME){
            InGameUIManager.instance.ToggleRunUI(false, false, false, false);
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