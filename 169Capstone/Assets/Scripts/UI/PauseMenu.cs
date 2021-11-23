using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject areYouSurePanel;

    public void ResumeGame()
    {
        ResetPauseUI();
        Time.timeScale = 1f;
        GameIsPaused = false;
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
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ApplySettingsChange()
    {
        Debug.Log("Applying settings changes...");
        settingsMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void CancelSettingsChange()
    {
        Debug.Log("No settings changes applied.");
        settingsMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}

/*
    New Inventory Plan
    ==================
    - use TABS and tab groups for the expanded item UI
        -> default panel where none of them are expanded
        -> a separate panel for each where one is expanded
        -> instead of on click, they swap on hover (over the small card)
            -> for controller support, that would be on select (not a button press, just moving to it with the stick)
        -> this might get weird but it's worth a shot!






    UI TODO:
    ========
    - redo death screen, pause menu, dialogue UI, and main menu with new UI grid stuff
    - compare item UI
    - once we implement saving, "are you sure" panel should mention if your progress was saved recently or whatever
    - lots of "TODO"s everywhere in different UI manager files
    - settings menu stuff (make it on either main menu or pause menu and then basically copy everything over from there to the other)
        -> volume (separated master(?), music, SFX)
        -> dimensions/resolution/full screen or otherwise whatever
        -> ???
        -> if we use the new input system we could have custom keybindings which i could handle :)
    - change where player pauses the game (in a more permanent player script)
        -> new input system would affect this
    - all UI input in fact
        -> including dialogue stuff
        -> make it so you don't have to physically click the button on the screen (click anywhere on screen/space/A button)
    - most UI value stuff (items especially)
*/
