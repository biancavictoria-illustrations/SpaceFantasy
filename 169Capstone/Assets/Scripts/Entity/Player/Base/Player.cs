using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public PlayerStats stats {get; private set;}
    public EntityHealth health {get; private set;}

    [SerializeField] public Transform handPos;

    //public Timer timer;
    //public bool test = true;

    [SerializeField] private SpeakerData speakerData;

    void Awake()
    {
        if( instance ){
            Debug.Log(instance);
            Destroy(gameObject);
        }
        else{
            instance = this;
            Debug.Log(instance);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Commenting this out because currently the Player is spawned after generation is already complete, causing StartOnGenerationCOmplete to not be run
        
        // if(GameManager.instance.InSceneWithRandomGeneration()){
        //     FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(StartOnGenerationComplete);
        // }
        // else{
        //     StartOnGenerationComplete();
        // }  

        StartOnGenerationComplete();      
    }

    private void StartOnGenerationComplete()
    {
        stats = GetComponent<PlayerStats>();

        health = gameObject.GetComponent<EntityHealth>();
        health.maxHitpoints = stats.getMaxHitPoints();
        health.currentHitpoints = stats.getMaxHitPoints();
        health.SetStartingHealthUI();

        StartCoroutine(DetectFall());

        FindObjectOfType<DevPanel>().UpdateValuesThatPersistBetweenScenes();    // TEMP -> REMOVE FOR FINAL BUILD

        // If your 1st or 2nd run, auto trigger starting dialogue
        if(GameManager.instance.currentRunNumber == 1 || (GameManager.instance.currentSceneName == GameManager.GAME_LEVEL1_STRING_NAME && GameManager.instance.currentRunNumber == 2)){
            GameManager.instance.inElevatorAnimation = true;
            FindObjectOfType<ElevatorAnimationHelper>().AddListenerToAnimationEnd( () => {
                StartAutoDialogueFromPlayer();
                GameManager.instance.inElevatorAnimation = false;
            });
        }
    }

    public void StartAutoDialogueFromPlayer(float timeToWait = GameManager.DEFAULT_AUTO_DIALOGUE_WAIT_TIME)
    {
        StartCoroutine(GameManager.instance.AutoRunDialogueAfterTime());
        
        if( GameManager.instance.currentRunNumber == 2 ){
            AlertTextUI.instance.EnableViewStatsAlert();
            StartCoroutine(AlertTextUI.instance.RemoveAlertAfterSeconds());
        }
    }

    private IEnumerator DetectFall()
    {
        yield return new WaitUntil(() => transform.position.y <= -6);

        if(health.tempPlayerGodModeToggle){
            health.tempPlayerGodModeToggle = false;
        }

        health.Damage(health.maxHitpoints, DamageSourceType.DeathPit);
    }

    public SpeakerData GetSpeakerData()
    {
        return speakerData;
    }
}
