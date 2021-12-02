using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main Hub");
        // TODO: Load save files
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game!");
    }
}
