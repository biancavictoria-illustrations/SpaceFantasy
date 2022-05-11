using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionDoor : MonoBehaviour
{
    public static SceneTransitionDoor ActiveDoor {get; private set;}

    [SerializeField] [Tooltip("The scene this door leads to.")]
    private string goToSceneName;    // Set in the inspector, the room this door LEADS TO

    [SerializeField] private ElevatorAnimationHelper elevatorHelper;

    void Start()
    {
        if(GameManager.instance.InSceneWithRandomGeneration())
        {
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(() => elevatorHelper.StartEnterAnimation());
        }
    }

    public void ChangeScene()
    {
        GameManager.instance.inElevatorAnimation = true;
        elevatorHelper.AddListenerToAnimationEnd(() => {
            GameManager.instance.inElevatorAnimation = false;
            SceneManager.LoadScene(goToSceneName);
        });
        elevatorHelper.StartExitAnimation();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveDoor = this;
            AlertTextUI.instance.EnableProceedDoorAlert();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveDoor = null;
            AlertTextUI.instance.DisableAlert();
        }
    }
}