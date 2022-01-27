using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private EnemyStats stats;
    private bool windUpRunning = false;
    private EntityHealth health;
    private float currentHitPoints = 0;
    public EnemyLogic logic;
    [HideInInspector] public Pathing path;
    [HideInInspector] public EntityAttack baseAttack;
    [HideInInspector] public bool canAttack = true;
    private GameManager gameManager;
    public GameObject timerPrefab;
    [HideInInspector] public bool coroutineRunning = false;
    [SerializeField] protected Animator animator;

    protected abstract IEnumerator EnemyLogic(); 

    // Start is called before the first frame update
    void Start()
    {
        path = gameObject.GetComponent<Pathing>();
        stats = gameObject.GetComponent<EnemyStats>();
        baseAttack = gameObject.GetComponent<EntityAttack>();
        health = gameObject.GetComponent<EntityHealth>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if(stats)
            stats.initializeStats();

        path.speed = stats.getMoveSpeed();
        path.provokedRadius = logic.provokedRange;
        path.attackRadius = logic.attackRange;

        // need to set up enemy health in here
        health.maxHitpoints = stats.getMaxHitPoints();
        //Debug.Log("Hitpoints = " + health.maxHitpoints.ToString());
        health.currentHitpoints = stats.getMaxHitPoints();

    }

    public void Update()
    {
        canAttack = !gameManager.inShopMode;

        if((path.Provoked() || path.InAttackRange()) && !coroutineRunning && canAttack) // Update for damage later
        {
            coroutineRunning = true;
            StartCoroutine(EnemyLogic());
        }

        if(windUpRunning && currentHitPoints > health.currentHitpoints)
        {
            animator.SetBool("WindUpInterrupted", true);
            SetCooldown();
        }
    }

    public void SetCooldown()
    {
        animator.SetBool("InCoolDown", true);
        path.attacking = false;
        StartCoroutine(RunCoolDownTimer());
    }

    private IEnumerator RunCoolDownTimer()
    {
        yield return new WaitForSeconds(logic.coolDown);
        animator.SetBool("InCoolDown", false);
        animator.SetBool("WindUpInterrupted", false);
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
