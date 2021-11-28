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
    - pause menu opens BEHIND dialogue UI
        -> is there a way to just set it to open on top of literally anything and everything else?

    - redo death screen, pause menu, dialogue UI, and main menu with new UI grid stuff?

    - compare item UI
        -> get the sizing right on the inventory screen before this so that we can just copy that and then mess with it
    - the rightmost part of the item cards isn't interactable??? just the part around the rarity/type

    - UI alerts that your game has saved automatically, or a manual save button

    - lots of "TODO"s everywhere in different UI manager files
    
    - change where player pauses the game (in a more permanent player script)
        -> new input system would affect this  
    - all UI input in fact
        -> including dialogue stuff
        -> make it so you don't have to physically click the button on the screen (click anywhere on screen/space/A button)
    
    - most UI value stuff (items especially)
*/
