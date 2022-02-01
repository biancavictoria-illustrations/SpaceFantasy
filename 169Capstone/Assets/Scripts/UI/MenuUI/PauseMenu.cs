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
    public GameObject areYouSurePanel;

    public Button continueButton;

    public void ResumeGame()
    {
        ResetPauseUI();
        Time.timeScale = 1f;
        GameIsPaused = false;

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
        areYouSurePanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        
        pauseMenuUI.SetActive(false);
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
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
        if(NPC.ActiveNPC.speakerData.SpeakerID() == SpeakerID.Bryn){
            InGameUIManager.instance.brynShopUI.SetShopUIInteractable(set);
        }
        else if(NPC.ActiveNPC.speakerData.SpeakerID() == SpeakerID.Stellan){
            InGameUIManager.instance.stellanShopUI.SetShopUIInteractable(set);
        }
        else if(NPC.ActiveNPC.speakerData.SpeakerID() == SpeakerID.Doctor){
            InGameUIManager.instance.doctorShopUI.SetShopUIInteractable(set);
        }
        else if(NPC.ActiveNPC.speakerData.SpeakerID() == SpeakerID.Andy){
            InGameUIManager.instance.weaponsShopUI.SetShopUIInteractable(set);
        }
        else{
            Debug.LogError("Failed to access shop data for NPC " + NPC.ActiveNPC.speakerData.SpeakerID());
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}

/*
    UI TODO:
    ========
    - lots of "TODO"s everywhere in different UI scripts

    - once saving exists: UI alert about how recently you saved in the "are you sure you want to quit" popup

    - game -> pause -> main menu -> play -> can't move anymore (trying to access dead animator???)
        -> was this fixed???
*/




/*
    Future Polish TODO:
    ===================
    - the rightmost part of the item cards isn't interactable??? just the part around the rarity/type
*/