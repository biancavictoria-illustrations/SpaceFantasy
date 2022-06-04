using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    [SerializeField] private ControlsMenu controlsMenu;

    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject controlsMenuPanel;
    public GameObject areYouSurePanel;

    public Button continueButton;
    public Button settingsButton;
    public Button controlsButton;
    public Button quitButton;

    public Button areYouSureNoButton;

    public void ResumeGame(bool unpauseWithEsc = false)
    {
        ResetPauseUI();
        GameManager.instance.pauseMenuOpen = false;
        GameIsPaused = false;

        if( GameManager.instance.InSceneWithGameTimer() ){
            InputManager.instance.RunGameTimer(true);
        }        

        if(InGameUIManager.instance.inventoryIsOpen){
            InGameUIManager.instance.inventoryUI.SetInventoryInteractable(true);
        }
        if(InGameUIManager.instance.gearSwapIsOpen){
            InGameUIManager.instance.gearSwapUI.SetSwapUIInteractable(true);
        }
        if(InputManager.instance.shopIsOpen){
            SetOpenShopUIInteractable(true);
        }

        if(unpauseWithEsc)
            controlsMenu.UpdateControlAlertUI();
    }

    private void ResetPauseUI()
    {
        settingsMenuPanel.SetActive(false);
        controlsMenuPanel.SetActive(false);
        areYouSurePanel.SetActive(false);
        pauseMenuPanel.SetActive(true);

        // In case we click escape to leave while the "are you sure you want to quit" panel is open
        TogglePauseMenuButtonInteractability(true);
        
        pauseMenuUI.SetActive(false);
    }

    public void PauseGame()
    {
        if( GameManager.instance.InSceneWithGameTimer() ){
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
        if(InputManager.instance.shopIsOpen){
            SetOpenShopUIInteractable(false);
        }

        // Quitting mid-dialogue causes problems so just disable quitting if in dialogue
        if(InputManager.instance.isInDialogue){
            quitButton.interactable = false;
        }
        else{
            quitButton.interactable = true;
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
        PlayerInventory.instance.ClearRunInventory();
        
        GameManager.instance.pauseMenuOpen = false;
        GameIsPaused = false;

        if( GameManager.instance.currentSceneName == GameManager.MAIN_HUB_STRING_NAME ){
            GameManager.instance.SaveGame();
        }

        SceneManager.LoadScene("MainMenu");
    }

    private void TogglePauseMenuButtonInteractability(bool set)
    {
        continueButton.interactable = set;
        settingsButton.interactable = set;
        controlsButton.interactable = set;
        quitButton.interactable = set;
    }

    public void ToggleAreYouSurePanelOn(bool set)
    {
        areYouSurePanel.SetActive(set);
        TogglePauseMenuButtonInteractability(!set);
        
        if(set){
            areYouSureNoButton.Select();
        }
        else{
            continueButton.Select();
        }
    }

    public void AreYouSureQuitButtonClicked()
    {
        ResetPauseUI();
        LoadMenu();
    }
}