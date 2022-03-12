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

    // TODO (call this when you start a new save file)
    public void StartNewGame()
    {
        // If starting a new game, load level 1 scene (new game)
        SceneManager.LoadScene(GameManager.GAME_LEVEL1_STRING_NAME);
    }

    // TODO (call this when you select your save file)
    public void LoadSavedGame()
    {
        GameManager.instance.LoadGame();
        SceneManager.LoadScene(GameManager.MAIN_HUB_STRING_NAME);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game!");
    }
}
