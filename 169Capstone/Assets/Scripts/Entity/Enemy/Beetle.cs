using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Enemy
{
    [SerializeField] private EnemyLogic phase1logic;
    [SerializeField] private EnemyLogic phase2logic;

    private Player player;
    private MeleePathing meleePath;
    private RangedPathing rangedPath;
    private Dictionary<AttackLogic, Pathing> attackToPathType;
    private Dictionary<AttackLogic, string> attackToAnimationTrigger;
    private bool isPhase2;

    protected override void Start()
    {
        player = FindObjectOfType<Player>();

        EntityHealth healthScript = GetComponent<EntityHealth>();
        healthScript.OnHit.AddListener(checkForHalfHealth);

        logic = phase1logic;

        meleePath = GetComponent<MeleePathing>();
        meleePath.enabled = false;
        meleePath.speed = stats.getMoveSpeed();
        meleePath.provokedRadius = logic.provokedRange;

        rangedPath = GetComponent<RangedPathing>();
        rangedPath.enabled = false;
        rangedPath.speed = stats.getMoveSpeed();
        rangedPath.provokedRadius = logic.provokedRange;
        rangedPath.nearAttackRange = 3;

        attackToPathType = new Dictionary<AttackLogic, Pathing>();
        attackToPathType.Add(logic.attacks[0], rangedPath);
        attackToPathType.Add(logic.attacks[1], meleePath);
        attackToPathType.Add(logic.attacks[2], rangedPath);

        attackToAnimationTrigger = new Dictionary<AttackLogic, string>();
        attackToAnimationTrigger.Add(logic.attacks[0], "isCharge");
        attackToAnimationTrigger.Add(logic.attacks[1], "isShockwave");
        attackToAnimationTrigger.Add(logic.attacks[2], "isSlam");

        nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)];

        path = attackToPathType[nextAttack];
        path.enabled = true;
        path.attackRadius = nextAttack.attackRange;
    }

    protected override IEnumerator EnemyLogic() //special
    {
        animator.SetBool("isMoving", true);
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        
        animator.SetBool("isMoving", false);
        path.attacking = true;

        if(attackToAnimationTrigger[nextAttack] == "isCharge")
        {
            animator.SetBool("isCharge", true);
            ChargeAttack();
        }
        else
        {
            animator.SetTrigger(attackToAnimationTrigger[nextAttack]);
        }
    }

    protected override IEnumerator RunCoolDownTimer()
    {
        yield return new WaitForSeconds(nextAttack.coolDown);
        animator.SetBool("InCoolDown", false);
        animator.SetBool("WindUpInterrupted", false);

        path.enabled = false;
        nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)];
        path = attackToPathType[nextAttack];
        path.attackRadius = nextAttack.attackRange;
        path.enabled = true;

        coroutineRunning = false;
    }

    private void ChargeAttack()
    {
        StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        path.enabled = false;
        path.agent.enabled = false;
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0;
        direction = direction.normalized;
        float speed = stats.getMoveSpeed() * 2;
        transform.rotation = Quaternion.LookRotation(direction);

        yield return new WaitForSeconds(nextAttack.windUp);

        RaycastHit hit;
        while(!Physics.SphereCast(transform.position, 3f, transform.forward, out hit, 0.5f, LayerMask.GetMask("Environment", "Player")))
        {
            yield return new WaitForFixedUpdate();
            transform.position += direction * speed * Time.fixedDeltaTime;
        }

        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player.GetComponent<EntityHealth>().Damage(logic.baseDamage * nextAttack.damageMultiplier);
            player.GetComponent<Movement>().ApplyExternalVelocity(direction * speed * 2);
        }

        path.agent.enabled = true;
        path.enabled = true;
        animator.SetBool("isCharge", false);
        SetCooldown();
    }

    private void MoveToPhase2()
    {
        logic = phase2logic;
        isPhase2 = true;

        attackToPathType = new Dictionary<AttackLogic, Pathing>();
        attackToPathType.Add(logic.attacks[0], rangedPath);
        attackToPathType.Add(logic.attacks[1], meleePath);
        attackToPathType.Add(logic.attacks[2], rangedPath);
        attackToPathType.Add(logic.attacks[3], rangedPath);
        
        attackToAnimationTrigger = new Dictionary<AttackLogic, string>();
        attackToAnimationTrigger.Add(logic.attacks[0], "isCharge");
        attackToAnimationTrigger.Add(logic.attacks[1], "isShockwave");
        attackToAnimationTrigger.Add(logic.attacks[2], "isSlam");
        attackToAnimationTrigger.Add(logic.attacks[3], "isArcaneMissiles");
    }

    private void checkForHalfHealth(EntityHealth health, float damage)
    {
        if(!isPhase2 && health.currentHitpoints <= health.maxHitpoints/2)
            MoveToPhase2();
    }
}
