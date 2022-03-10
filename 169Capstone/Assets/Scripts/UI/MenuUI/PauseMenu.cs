using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject controlsMenuPanel;
    public GameObject areYouSurePanel;

    public Button continueButton;
    public Button settingsButton;
    public Button controlsButton;
    public Button quitButton;

    public void ResumeGame()
    {
        ResetPauseUI();
        GameManager.instance.pauseMenuOpen = false;
        GameIsPaused = false;

        if( GameManager.instance.currentSceneName != GameManager.MAIN_HUB_STRING_NAME ){
            InputManager.instance.RunGameTimer(true);
        }        

        if(InGameUIManager.instance.inventoryIsOpen){
            InGameUIManager.instance.inventoryUI.SetInventoryInteractable(true);
        }
        if(InGameUIManager.instance.gearSwapIsOpen){
            InGameUIManager.instance.gearSwapUI.SetSwapUIInteractable(true);
        }
        if(InputManager.instance.isInDialogue){
            // Select the next button
            DialogueManager.instance.nextButton.interactable = true;
            DialogueManager.instance.nextButton.Select();
        }
        if(InputManager.instance.shopIsOpen){
            SetOpenShopUIInteractable(true);
        }
    }

    private void ResetPauseUI()
    {
        settingsMenuPanel.SetActive(false);
        controlsMenuPanel.SetActive(false);
        areYouSurePanel.SetActive(false);
        pauseMenuPanel.SetActive(true);

        // In case we click escape to leave while the "are you sure you want to quit" panel is open
        continueButton.interactable = true;
        settingsButton.interactable = true;
        controlsButton.interactable = true;
        quitButton.interactable = true;
        
        pauseMenuUI.SetActive(false);
    }

    public void PauseGame()
    {
        if( GameManager.instance.currentSceneName != GameManager.MAIN_HUB_STRING_NAME ){
            InputManager.instance.RunGameTimer(false);
        }

        pauseMenuUI.SetActive(true);
        GameManager.instance.pauseMenuOpen = true;
        GameIsPaused = true;
        continueButton.Select();

        if(InGameUIManager.instance.inventoryIsOpen){
            InGameUIManager.instance.inventoryUI.SetInventoryInteractable(false);
        }
        if(InGameUIManager.instance.gearSwapIsOpen){
            InGameUIManager.instance.gearSwapUI.SetSwapUIInteractable(false);
        }
        if(InputManager.instance.isInDialogue){
            DialogueManager.instance.nextButton.interactable = false;
        }
        if(InputManager.instance.shopIsOpen){
            SetOpenShopUIInteractable(false);
        }
    }

    private void SetOpenShopUIInteractable(bool set)
    {
        if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Bryn){
            InGameUIManager.instance.brynShopUI.SetShopUIInteractable(set);
        }
        else if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Stellan){
            InGameUIManager.instance.stellanShopUI.SetShopUIInteractable(set);
        }
        else if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Doctor){
            InGameUIManager.instance.doctorShopUI.SetShopUIInteractable(set);
        }
        else if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Rhian){
            InGameUIManager.instance.weaponsShopUI.SetShopUIInteractable(set);
        }
        else{
            Debug.LogError("Failed to access shop data for NPC " + NPC.ActiveNPC.SpeakerData().SpeakerID());
        }
    }

    public void LoadMenu()
    {
        GameManager.instance.pauseMenuOpen = false;
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}


// TODO: fix pause menu now that dialogue pauses (if you open pause menu while dialogue open)