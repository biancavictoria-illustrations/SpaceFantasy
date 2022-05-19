using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    public GameObject deathScreenUI;
    public TMP_Text deathMessage;

    public Button continueButton;

    public const string deathMessage0 = "Resistance is futile. But Atlan resisted nonetheless.";
    public const string deathMessage1 = "The stubborn, handsome scoundrel once more met his end... only to return.";
    public const string deathMessage2 = "And so the spacefaring adventurer met yet another end, only to find himself back at the beginning once more.";

    public void OpenPlayerDeathUI()
    {
        SetDeathMessage();
        deathScreenUI.SetActive(true);
        InGameUIManager.instance.SetGameUIActive(false);
        GameManager.instance.deathMenuOpen = true;
        continueButton.Select();
    }

    public void ClosePlayerDeathUI()
    {
        deathScreenUI.SetActive(false);
        InGameUIManager.instance.SetGameUIActive(true);
    }

    private void SetDeathMessage()
    {
        if(GameManager.instance.currentRunNumber == 1){
            deathMessage.text = "And so the spacefaring adventurer met his end.\n...Or so he thought.";
            return;
        }

        int num = Random.Range(0,2);
        switch(num){
            case 0:
                deathMessage.text = deathMessage0;
                return;
            case 1:
                deathMessage.text = deathMessage1;
                return;
            case 2:
                deathMessage.text = deathMessage2;
                return;
        }
    }

    // Called when player clicks continue button on death screen UI
    public void GoToMainHubOnDeath()
    {
        GameManager.instance.deathMenuOpen = false;
        Billboard.generationComplete = false;
        GameManager.instance.EndRun();
        SceneManager.LoadScene(GameManager.MAIN_HUB_STRING_NAME);
    }
}
