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
        GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
}

/*
    UI TODO:
    ========
    - compare item UI

    - shop UI
    - UI interact alerts

    - UI item values
    - lots of "TODO"s everywhere in different UI scripts

    - default inventory values (if you have nothing in a slot)

    - once saving exists: UI alert about how recently you saved in the "are you sure you want to quit" popup

    - game -> pause -> main menu -> play -> can't move anymore (trying to access dead animator???)
        -> was this fixed???
*/




/*
    Future Polish TODO:
    ===================
    - the rightmost part of the item cards isn't interactable??? just the part around the rarity/type
*/