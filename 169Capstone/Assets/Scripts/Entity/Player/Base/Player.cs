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
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<PlayerStats>();

        health = gameObject.GetComponent<EntityHealth>();
        health.maxHitpoints = stats.getMaxHitPoints();
        health.currentHitpoints = stats.getMaxHitPoints();
        health.SetStartingHealthUI();

        StartCoroutine(DetectFall());

        // If your first run, auto trigger starting dialogue
        if(GameManager.instance.currentRunNumber == 1){
            StartAutoDialogueFromPlayer();
        }
    }

    public void StartAutoDialogueFromPlayer(float timeToWait = GameManager.DEFAULT_AUTO_DIALOGUE_WAIT_TIME)
    {
        StartCoroutine(GameManager.instance.AutoRunDialogueAfterTime());
    }

    private IEnumerator DetectFall()
    {
        yield return new WaitUntil(() => transform.position.y <= -6);
        health.Damage(health.maxHitpoints);
    }

    public SpeakerData GetSpeakerData()
    {
        return speakerData;
    }
}
