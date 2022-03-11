using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;

    void Start()
    {
        playButton.Select();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(GameManager.GAME_LEVEL1_STRING_NAME);
    }

    // TODO
    public void StartNewGame()
    {
        // if starting a new game, load level 1 scene (new game)
        SceneManager.LoadScene(GameManager.GAME_LEVEL1_STRING_NAME);
    }

    // TODO
    public void LoadSavedGame()
    {
        // TODO: Load save files and load main hub
        SceneManager.LoadScene(GameManager.MAIN_HUB_STRING_NAME);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game!");
    }
}
