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
        
        // TODO: Load save files
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game!");
    }
}
