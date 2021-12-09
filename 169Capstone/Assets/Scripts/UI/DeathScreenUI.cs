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
        Time.timeScale = 0f;
        continueButton.Select();
    }

    private void SetDeathMessage()
    {
        // TODO: Fill this in!
        deathMessage.text = "You died!";
    }

    // TODO: Add all the reset stuff here too
    // Including possibly dialogue reset stuff & inventory stuff
    public void GoToMainHub()
    {
        Time.timeScale = 1f;
        GameManager.instance.EndRun();
        SceneManager.LoadScene("Main Hub");
    }
}
