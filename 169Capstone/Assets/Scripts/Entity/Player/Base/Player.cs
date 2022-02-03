using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject[] gear = new GameObject[4];
    [SerializeField] private string weapon;
    [SerializeField] private string accessory;
    [SerializeField] private string hand;
    [SerializeField] private string legs;
    [SerializeField] private PlayerStats stats;
    private EntityHealth health;

    public int currentStr;
    public int currentDex;
    public int currentCon;
    public int currentInt;
    public int currentWis;
    public int currentCha;

    public float currentAttackSpeed;
    //private float currentHitpoints;

    //public Timer timer;

    //public ObjectManager objectManager;

    //public bool test = true;

    [SerializeField] private SpeakerData speakerData;

    // Start is called before the first frame update
    void Start()
    {
        weapon = "Berserker's Zweihander";
        //gear[0] = objectManager.GetGearObject(weapon);
        //gear[0] = GameManager.instance.GetGearObject(weapon, "Weapon");
        health = gameObject.GetComponent<EntityHealth>();
        health.maxHitpoints = 30;
        health.currentHitpoints = 30;
        //Debug.Log("here");
        //health.maxHitpoints = stats.getMaxHitPoints();
        //health.currentHitpoints = stats.getMaxHitPoints();

        Debug.Log(health.maxHitpoints);

        currentStr = stats.Strength();
        currentDex = stats.Dexterity();
        currentCon = stats.Constitution();
        currentInt = stats.Intelligence();
        currentWis = stats.Wisdom();
        currentCha = stats.Charisma();
        currentAttackSpeed = stats.getAttackSpeed();
        //currentHitpoints = stats.getMaxHitPoints();

        if(DialogueManager.instance != null && !DialogueManager.instance.DialogueManagerHasSpeaker(speakerData)){
            DialogueManager.instance.AddSpeaker(speakerData);
        }

        StartCoroutine(DetectFall());
    }

    // Update is called once per frame
    void Update()
    {
        if(gear[0] == null)
        {
            Debug.Log("No Weapon");
            // gear[0] = GameManager.instance.GetGearObject(weapon, "Weapon");
        }
        /*if (Input.GetButtonDown("Fire1"))
        {
            attack.Attack(timer, true, 10, 10, 10);
        }*/
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
