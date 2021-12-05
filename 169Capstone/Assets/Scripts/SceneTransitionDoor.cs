using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionDoor : MonoBehaviour
{
    public static SceneTransitionDoor ActiveDoor {get; private set;}

    [SerializeField] private string goToSceneName;    // Set in the inspector, the room this door LEADS TO


    public void ChangeScene()
    {
        SceneManager.LoadScene(goToSceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveDoor = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveDoor = null;
        }
    }
}


/*
    InputManager bottom of OnInteract function

    // If you're in range of a door, walk through it
    else if(SceneTransitionDoor.ActiveDoor){
        SceneTransitionDoor.ActiveDoor.ChangeScene();
    }
*/