using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionDoor : MonoBehaviour
{
    public static SceneTransitionDoor ActiveDoor {get; private set;}

    [SerializeField] [Tooltip("The scene this door leads to.")]
    private string goToSceneName;

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

        FindObjectOfType<ScreenFade>().FadeOut(1f);
        
        elevatorHelper.AddListenerToAnimationEnd(() => {
            GameManager.instance.inElevatorAnimation = false;

            // If we're NOT going to the main hub, title screen, or the first level, save the player so that their build is retained
            if(goToSceneName != GameManager.MAIN_HUB_STRING_NAME && goToSceneName != GameManager.GAME_LEVEL1_STRING_NAME && goToSceneName != GameManager.TITLE_SCREEN_STRING_NAME){
                Player.instance.transform.parent = GameManager.instance.transform;
            }
            // If we ARE going to title screen, clear run inventory
            if( goToSceneName == GameManager.TITLE_SCREEN_STRING_NAME ){
                PlayerInventory.instance.ClearRunInventory();
            }

            SceneManager.LoadScene(goToSceneName);
        });
        elevatorHelper.StartExitAnimation();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveDoor = this;

            if(GameManager.instance.currentSceneName != GameManager.EPILOGUE_SCENE_STRING_NAME)
                AlertTextUI.instance.EnableContinueDoorAlert();
            else
                AlertTextUI.instance.EnableLeaveDoorAlert();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the collision was caused by the player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActiveDoor = null;
            AlertTextUI.instance.DisablePrimaryAlert();
        }
    }
}