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
        if(InputManager.instance.isInDialogue){
            // Select the next button
            DialogueManager.instance.nextButton.interactable = true;
            DialogueManager.instance.nextButton.Select();
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
        if(InputManager.instance.isInDialogue){
            DialogueManager.instance.nextButton.interactable = false;
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

/*
    UI TODO:
    ========
    - compare item UI
    - shop UI

    - UI item values, currency values, potion values, etc.
    
    - pause menu opens BEHIND dialogue UI
        -> is there a way to just set it to open on top of literally anything and everything else?

    - redo dialogue UI and main menu with new UI grid stuff?
    
    - the rightmost part of the item cards isn't interactable??? just the part around the rarity/type (polish fix)

    - UI alert about how recently you saved in the "are you sure you want to quit" popup

    - lots of "TODO"s everywhere in different UI manager files
*/
