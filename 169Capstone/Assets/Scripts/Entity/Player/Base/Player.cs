using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] private PlayerStats stats;
    private EntityHealth health;

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
    public string playerYarnHeadNode {get; private set;}

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
        playerYarnHeadNode = speakerData.SpeakerID() + "Start";
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
        //currentHitpoints = stats.getMaxHitPoints();

        if(DialogueManager.instance != null && !DialogueManager.instance.DialogueManagerHasSpeaker(speakerData)){
            DialogueManager.instance.dialogueRunner.Add(speakerData.YarnDialogue());
            DialogueManager.instance.AddSpeaker(speakerData);
        }

        StartCoroutine(DetectFall());

        // TEMP drop the sword on start so the player has a working weapon (for testing purposes)
        GameObject itemObject = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        itemObject.GetComponent<GeneratedEquipment>().SetEquipmentBaseData( swordData, ItemRarity.Common );
        itemObject.GetComponent<DropTrigger>().DropItemModelIn3DSpace();

        // If your first run, auto trigger starting dialogue
        if(GameManager.instance.currentRunNumber == 1){
            StartCoroutine(GameManager.instance.AutoRunDialogueAfterTime());
        }
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
}
