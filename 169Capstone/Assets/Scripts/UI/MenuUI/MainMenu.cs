using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [SerializeField] private Button playButton;
    [SerializeField] private GameObject mainMenuPanel;

    [SerializeField] private GameObject saveFilePanel;

    [SerializeField] private List<Button> fileSelectButtons;

    [SerializeField] private GameObject areYouSurePanel;
    [SerializeField] private Button areYouSureNoButton;
    private int activeDeletePanel = 0;

    [SerializeField] private GameObject titleScreenCanvas;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    void Start()
    {
        playButton.Select();
    }

    public void ToggleSaveFileMenu(bool set)
    {
        saveFilePanel.SetActive(set);
        mainMenuPanel.SetActive(!set);

        if(set){
            fileSelectButtons[0].Select();
        }
        else{
            playButton.Select();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetAllFileSelectButtonsInteractable(bool set)
    {
        foreach(Button b in fileSelectButtons){
            b.interactable = set;
        }
    }

    public void ToggleAreYouSureYouWantToDeleteSaveFilePanel(bool set, int _slot)
    {
        areYouSurePanel.SetActive(set);
        SetAllFileSelectButtonsInteractable(!set);
        activeDeletePanel = _slot;

        if(set){
            areYouSureNoButton.Select();
        }
        else{
            fileSelectButtons[0].Select();
        }
    }

    public void CancelDeleteButtonClicked()
    {
        ToggleAreYouSureYouWantToDeleteSaveFilePanel(false, 0);
    }

    public void DeleteFile()
    {
        foreach(Button b in fileSelectButtons){
            SaveFilePanel p = b.GetComponent<SaveFilePanel>();
            if( p && p.GetSlotNumber() == activeDeletePanel ){
                p.ClearSaveFile();
            }
        }

        GameManager.instance.DeleteSaveFile(activeDeletePanel);

        ToggleAreYouSureYouWantToDeleteSaveFilePanel(false, 0);
    }

    public void DeactivateMainMenuUI()
    {
        titleScreenCanvas.SetActive(false);
    }
}
