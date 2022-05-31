using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected const float healthIncreasePerTier = 0.2f;
    protected const float damageIncreasePerTier = 0.5f;

    public EnemyLogic logic;
    public GameObject timerPrefab;

    [HideInInspector] public Pathing path;
    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool coroutineRunning = false;

    [SerializeField] protected Animator animator;

    protected EnemyStats stats;
    protected EntityHealth health;
    protected AttackLogic nextAttack;

    protected bool windUpRunning = false;
    protected float currentHitPoints = 0;
    protected int currentTier;

    protected abstract IEnumerator EnemyLogic(); 

    protected virtual void Awake()
    {
        stats = GetComponent<EnemyStats>();
        health = GetComponent<EntityHealth>();

        if(stats)
            stats.initializeStats();

        float maxHitPoints = Mathf.FloorToInt(stats.getMaxHitPoints() * (1 + healthIncreasePerTier * currentTier));
        health.maxHitpoints = maxHitPoints;
        health.currentHitpoints = maxHitPoints;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        path = GetComponent<Pathing>();

        path.speed = stats.getMoveSpeed();
        path.provokedRadius = logic.provokedRange;

        nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)];
        path.attackRadius = nextAttack.attackRange;

        GameManager.instance.gameTimer.OnTierIncrease.AddListener(OnTierIncrease);
    }

    void Update()
    {
        path.canMove = canAttack;

        if(!coroutineRunning && canAttack && (path.Provoked() || path.InAttackRange())) // Update for damage later
        {
            coroutineRunning = true;
            StartCoroutine(EnemyLogic());
        }

        if(windUpRunning && currentHitPoints > health.currentHitpoints)
        {
            if(nextAttack.isInterruptible)
                animator.SetBool("WindUpInterrupted", true);
                
            SetCooldown();
        }
    }

    public virtual void SetCooldown()
    {
        animator.SetBool("InCoolDown", true);
        path.attacking = false;
        StartCoroutine(RunCoolDownTimer());
    }

    protected virtual IEnumerator RunCoolDownTimer()
    {
        yield return new WaitForSeconds(nextAttack.coolDown);
        animator.SetBool("InCoolDown", false);
        animator.SetBool("WindUpInterrupted", false);
        nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)];
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

    protected virtual void OnTierIncrease(int newTier)
    {
        currentTier = newTier;

        float newMaxHitPoints = Mathf.FloorToInt(stats.getMaxHitPoints() * (1 + healthIncreasePerTier * currentTier));
        float hitPointDiff = newMaxHitPoints - health.maxHitpoints;
        health.maxHitpoints = newMaxHitPoints;
        health.currentHitpoints += hitPointDiff;
        health.SetStartingHealthUI();
    }
}
