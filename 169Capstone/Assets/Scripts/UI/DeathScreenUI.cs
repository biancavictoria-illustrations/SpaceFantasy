using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathScreenUI : MonoBehaviour
{
    public GameObject deathScreenUI;
    public TMP_Text deathMessage;

    // TODO: Call this when the player dies
    public void OpenPlayerDeathUI()
    {
        SetDeathMessage();
        deathScreenUI.SetActive(true);
        Time.timeScale = 0f;
    }

    private void SetDeathMessage()
    {
        // TODO: Fill this in!
        deathMessage.text = "deth message";
    }

    // TODO: Add all the reset stuff here too
    // Including possibly dialogue reset stuff & inventory stuff
    public void GoToMainHub()
    {
        Time.timeScale = 1f;
        // SceneManager.LoadScene("MainHub");
        Debug.Log("Transporting player to main hub!");
    }
}
