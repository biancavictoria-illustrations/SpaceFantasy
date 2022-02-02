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

    public void OpenPlayerDeathUI()
    {
        SetDeathMessage();
        deathScreenUI.SetActive(true);
        InGameUIManager.instance.SetGameUIActive(false);
        Time.timeScale = 0f;
        continueButton.Select();
    }

    public void ClosePlayerDeathUI()
    {
        deathScreenUI.SetActive(false);
        InGameUIManager.instance.SetGameUIActive(true);
    }

    private void SetDeathMessage()
    {
        // TODO: Fill this in!
        deathMessage.text = "You died!";
    }

    // TODO: Add all the reset stuff here too (EndRun function necessary?)
    // Including possibly dialogue reset stuff & inventory stuff
    public void GoToMainHub()
    {
        Time.timeScale = 1f;
        ClosePlayerDeathUI();
        // GameManager.instance.EndRun();
        SceneManager.LoadScene("Main Hub");

    }
}
