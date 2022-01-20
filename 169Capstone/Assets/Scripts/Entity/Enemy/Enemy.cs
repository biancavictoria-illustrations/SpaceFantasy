using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyStats stats;
    private bool windUpRunning = false;
    private EntityHealth health;
    private float currentHitPoints = 0;
    public EnemyLogic logic;
    [HideInInspector] public GroundPathing path;
    [HideInInspector] public EntityAttack baseAttack;
    [HideInInspector] public bool canAttack = true;
    private GameManager gameManager;
    public GameObject timerPrefab;
    [HideInInspector] public bool coroutineRunning = false;
    [SerializeField] protected Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        path = gameObject.GetComponent<GroundPathing>();
        stats = gameObject.GetComponent<EnemyStats>();
        baseAttack = gameObject.GetComponent<EntityAttack>();
        health = gameObject.GetComponent<EntityHealth>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if(stats)
            stats.initializeStats();

        path.speed = stats.getMoveSpeed();
        //Debug.Log("Pathing speed = " + path.speed.ToString());
        //path.speed = 5;
        path.provokedRadius = logic.provokedRange;
        path.attackRadius = logic.attackRange;

        // need to set up enemy health in here
        health.maxHitpoints = stats.getMaxHitPoints();
        //Debug.Log("Hitpoints = " + health.maxHitpoints.ToString());
        health.currentHitpoints = stats.getMaxHitPoints();

    }

    public void Update()
    {
        if(gameManager.inShopMode)
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }

        if(windUpRunning && currentHitPoints > health.currentHitpoints)
        {
            animator.SetBool("WindUpInterrupted", true);
            SetCooldown();
        }
    }

    public IEnumerator CallDamage()
    {
        yield return new WaitUntil(() => baseAttack.hit);
        baseAttack.enemyDeath = DealDamage();
        baseAttack.damageDealt = true;
    }

    public bool DealDamage()
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit, logic.attackRange))
        {
            return hit.collider.tag == "Player" ? hit.collider.GetComponent<EntityHealth>().Damage(logic.damage) : false;
        }
        else
        {
            return false;
        }
    }

    public void SetCooldown()
    {
        animator.SetBool("InCoolDown", true);
        StartCoroutine(RunCoolDownTimer());
    }

    private IEnumerator RunCoolDownTimer()
    {
        Timer timer = Instantiate(timerPrefab).GetComponent<Timer>();
        timer.StartTimer(logic.coolDown);
        yield return new WaitUntil(() => timer.timeRemaining <= 0);
        Destroy(timer);
        animator.SetBool("InCoolDown", false);
        animator.SetBool("WindUpInterrupted", false);
        path.attacking = false;
        coroutineRunning = false;

    }

    public void EnableWindUpRunning()
    {
        windUpRunning = true;
        currentHitPoints = health.currentHitpoints;
    }

    public void DisableWindUpRunning()
    {
        windUpRunning = false;
    }
}
