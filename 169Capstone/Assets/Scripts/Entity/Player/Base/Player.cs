using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] private PlayerStats stats;
    public EntityHealth health {get; private set;}

    // Do we need these??? they're not being updated with the doctor shop upgrades i don't think so we should probably get rid of them cuz they're redundant with normal stats...?
    public int currentStr;
    public int currentDex;
    public int currentCon;
    public int currentInt;
    public int currentWis;
    public int currentCha;

    public float currentAttackSpeed;

    [SerializeField] public Transform handPos;

    //public Timer timer;
    //public bool test = true;

    [SerializeField] private SpeakerData speakerData;

    // TEMP for testing purposes
    public GameObject dropItemPrefab;
    public EquipmentBaseData swordData;

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
        health = gameObject.GetComponent<EntityHealth>();
        health.maxHitpoints = 30;
        health.currentHitpoints = 30;        
        //health.maxHitpoints = stats.getMaxHitPoints();
        //health.currentHitpoints = stats.getMaxHitPoints();
        health.SetStartingHealthUI();

        currentStr = stats.Strength();
        currentDex = stats.Dexterity();
        currentCon = stats.Constitution();
        currentInt = stats.Intelligence();
        currentWis = stats.Wisdom();
        currentCha = stats.Charisma();
        currentAttackSpeed = stats.getAttackSpeed();

        StartCoroutine(DetectFall());

        // TEMP drop the sword on start so the player has a working weapon (for testing purposes)
        if(GameManager.instance.currentSceneName != GameManager.MAIN_HUB_STRING_NAME){
            GameObject itemObject = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
            itemObject.GetComponent<GeneratedEquipment>().SetEquipmentBaseData( swordData, ItemRarity.Common );
            itemObject.GetComponent<DropTrigger>().DropItemModelIn3DSpace();
        }

        // If your first run, auto trigger starting dialogue
        if(GameManager.instance.currentRunNumber == 1){
            StartAutoDialogueFromPlayer();
        }
    }

    public void StartAutoDialogueFromPlayer(float timeToWait = GameManager.DEFAULT_AUTO_DIALOGUE_WAIT_TIME)
    {
        StartCoroutine(GameManager.instance.AutoRunDialogueAfterTime());
    }

    public float CurrentAttackSpeed()
    {
        return currentAttackSpeed;
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
