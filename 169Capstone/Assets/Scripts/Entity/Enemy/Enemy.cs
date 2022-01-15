using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyStats stats;
    private EntityHealth health;
    public EnemyLogic logic;
    [HideInInspector] public GroundPathing path;
    [HideInInspector] public EntityAttack baseAttack;
    [HideInInspector] public bool canAttack = true;
    private GameManager gameManager;
    public GameObject timerPrefab;
    public bool coroutineRunning = false;
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

        //path.speed = stats.getMoveSpeed();
        //Debug.Log("Pathing speed = " + path.speed.ToString());
        path.speed = 5;
        path.provokedRadius = logic.provokedRange;
        path.attackRadius = logic.attackRange;

        // need to set up enemy health in here
        health.maxHitpoints = stats.getMaxHitPoints();
        health.currentHitpoints = stats.getMaxHitPoints();

    }

    public void Update()
    {
        if(gameManager.inShopMode)
        {
            canAttack = false;
        }
    }

    public IEnumerator CallDamage()
    {
        yield return new WaitUntil(() => baseAttack.hit);
        baseAttack.enemyDeath = DealDamage();
        baseAttack.damageDealt = true;
    }

    private bool DealDamage()
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
}
