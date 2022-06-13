using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Lich : Enemy
{
    private delegate void CoroutineCallback();

    private const float phase1MeteorFrequency = 0.25f;
    private const float phase2MeteorFrequency = 0.125f;

    [SerializeField] private EnemyLogic phase1logic;
    [SerializeField] private EnemyLogic phase2logic;
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject robertPrefab;
    [SerializeField] private GameObject slowCirclePrefab;

    private Player player;
    private Coroutine attackRoutine;
    private Dictionary<AttackLogic, string> attackToAnimationTrigger;
    private HashSet<GameObject> livingEnemies;
    private bool isPhase2;
    private bool canSummon = true;
    private float meteorFrequency { get { return isPhase2 ? phase2MeteorFrequency : phase1MeteorFrequency; } }

    protected override void OnTierIncrease(int newTier) {}
    
    protected override void Start()
    {
        base.Start();

        canAttack = false;

        player = FindObjectOfType<Player>();

        EntityHealth healthScript = GetComponent<EntityHealth>();
        healthScript.OnHit.AddListener(checkForHalfHealth);
        healthScript.SetStartingHealthUI();

        logic = phase1logic;

        livingEnemies = new HashSet<GameObject>();

        attackToAnimationTrigger = new Dictionary<AttackLogic, string>();
        attackToAnimationTrigger.Add(logic.attacks[0], "isMagicMissile");
        attackToAnimationTrigger.Add(logic.attacks[1], "isMeteorShower");
        attackToAnimationTrigger.Add(logic.attacks[2], "isSummoning");

        this.currentTier = GameManager.instance.gameTimer.enemyTier;
        float newMaxHitPoints = Mathf.FloorToInt(stats.getMaxHitPoints() * (1 + healthIncreasePerTier * currentTier));
        float hitPointDiff = newMaxHitPoints - health.maxHitpoints;
        health.maxHitpoints = newMaxHitPoints;
        health.currentHitpoints += hitPointDiff;
        health.SetStartingHealthUI();
    }

    protected override IEnumerator EnemyLogic() //special
    {
        animator.SetBool("isMoving", true);
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        
        animator.SetBool("isMoving", false);
        path.attacking = true;

        if(!attackToAnimationTrigger.ContainsKey(nextAttack))
            nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)];

        animator.SetTrigger(attackToAnimationTrigger[nextAttack]);
    }

    public override void SetCooldown()
    {
        if(attackRoutine != null)
            StopCoroutine(attackRoutine);

        base.SetCooldown();
    }

    protected override IEnumerator RunCoolDownTimer()
    {
        yield return new WaitForSeconds(nextAttack.coolDown);
        animator.SetBool("InCoolDown", false);
        animator.SetBool("WindUpInterrupted", false);

        path.enabled = false;
        AttackLogic previousAttack = nextAttack;

        int attackCount = canSummon ? logic.attacks.Count : logic.attacks.Count-1;

        nextAttack = logic.attacks[Random.Range(0, attackCount)];
        if(previousAttack == nextAttack)
            nextAttack = logic.attacks[Random.Range(0, attackCount)]; //Reroll next attack to try to prevent duplicate attacks

        path.attackRadius = nextAttack.attackRange;
        path.enabled = true;

        coroutineRunning = false;
    }

    public void MagicMissile()
    {
        Vector3 towardsPlayer = player.transform.position - transform.position;
        towardsPlayer.y = 0;
        GameObject missile = Instantiate(missilePrefab, transform.position + transform.up + transform.forward, Quaternion.FromToRotation(transform.position, player.transform.position));
        missile.GetComponent<Projectile>().Initialize("Player", logic.baseDamage * nextAttack.damageMultiplier * (1 + damageIncreasePerTier * currentTier), DamageSourceType.TimeLich, towardsPlayer);
    }

    public void MeteorShower()
    {
        attackRoutine = StartCoroutine(MeteorRoutine()); 
    }

    public void SinisterSummons()
    {
        canSummon = false;

        int numEnemies = isPhase2 ? 4 : 2;
        numEnemies += Random.Range(0, 2);

        for(int i = 0; i < numEnemies; ++i)
        {
            GameObject prefab = Random.Range(0, 2) == 0 ? slimePrefab : robertPrefab;
            GameObject enemy = Instantiate(prefab, transform.position, Quaternion.identity);
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            Vector3 spawnPosition = transform.position;
            Vector3 newPosition;
            NavMeshPath path = new NavMeshPath();
            int count = 0;
            do
            {
                float angle = Random.Range(0, Mathf.PI * 2);
                float magnitude = Random.Range(5f, 30f);
                newPosition = spawnPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * magnitude;
                ++count;
            }
            while (count < 100 && !agent.CalculatePath(newPosition, path));
            agent.Warp(newPosition);

            livingEnemies.Add(enemy);
            enemy.GetComponent<EntityHealth>().OnDeath.AddListener( (EntityHealth healthScript) => { minionWasKilled(enemy); } );
        }
    }

    public void TimeDilation()
    {
        SlowCircle slowScript = Instantiate(slowCirclePrefab, player.transform.position, Quaternion.identity).GetComponent<SlowCircle>();
        slowScript.Initialize(this, "Player", "Enemy", 0.5f, 10, radius: 8, fadeInDuration: 1);
    }

    private IEnumerator MeteorRoutine()
    {
        path.enabled = false;
        int count = 0;
        while(count < nextAttack.duration/meteorFrequency)
        {
            GameObject meteor = Instantiate(meteorPrefab, player.transform.position, Quaternion.identity);
            meteor.GetComponent<FallingMeteor>().damage = logic.baseDamage * nextAttack.damageMultiplier * (1 + damageIncreasePerTier * currentTier);
            ++count;
            yield return new WaitForSeconds(meteorFrequency);
        }
        path.enabled = true;
    }

    private void MoveToPhase2()
    {
        logic = phase2logic;
        isPhase2 = true;
        animator.SetBool("isPhase2", true);
        
        attackToAnimationTrigger.Clear();
        attackToAnimationTrigger.Add(logic.attacks[0], "isMagicMissile");
        attackToAnimationTrigger.Add(logic.attacks[1], "isMeteorShower");
        attackToAnimationTrigger.Add(logic.attacks[2], "isTimeDilation");
        attackToAnimationTrigger.Add(logic.attacks[3], "isSummoning");
    }

    private void minionWasKilled(GameObject enemy)
    {
        livingEnemies.Remove(enemy);
        if(livingEnemies.Count == 0)
            canSummon = true;
    }

    private void checkForHalfHealth(EntityHealth health, float damage)
    {
        if(!isPhase2 && health.currentHitpoints <= health.maxHitpoints/2)
            MoveToPhase2();
    }
}
