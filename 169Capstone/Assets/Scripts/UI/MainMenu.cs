using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // SceneManager.LoadScene("Game");
        // Do we have save files??? Probably should. Gonna have to deal with that here
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
