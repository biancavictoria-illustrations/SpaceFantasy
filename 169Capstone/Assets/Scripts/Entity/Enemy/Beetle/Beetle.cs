﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Enemy
{
    private delegate void CoroutineCallback();

    private const float phase1SlamFrequency = 0.25f;
    private const float phase2SlamFrequency = 0.125f;

    [SerializeField] private EnemyLogic phase1logic;
    [SerializeField] private EnemyLogic phase2logic;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private GameObject hurtCirclePrefab;

    private Player player;
    private Coroutine attackRoutine;
    private MeleePathing meleePath;
    private RangedPathing rangedPath;
    private Dictionary<AttackLogic, Pathing> attackToPathType;
    private Dictionary<AttackLogic, string> attackToAnimationTrigger;
    private HurtCircle hurtScript;
    private bool isPhase2;
    private float slamDebrisFrequency { get { return isPhase2 ? phase2SlamFrequency : phase1SlamFrequency; } }
    private int shockwaveCount;

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
        shockwaveCount = 0;

        if(!attackToAnimationTrigger.ContainsKey(nextAttack))
            nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)];

        if(attackToAnimationTrigger[nextAttack] == "isCharge")
        {
            animator.SetBool("isCharge", true);
            ChargeAttack();
        }
        else if(attackToAnimationTrigger[nextAttack] == "isArcaneMissiles")
        {
            path.enabled = false;
            StartCoroutine(turnTowardsPlayerRoutine(nextAttack.windUp, callback: () => animator.SetTrigger("isArcaneMissiles")));
        }
        else
        {
            animator.SetTrigger(attackToAnimationTrigger[nextAttack]);
        }
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

        nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)];
        if(previousAttack == nextAttack)
            nextAttack = logic.attacks[Random.Range(0, logic.attacks.Count)]; //Reroll next attack to try to prevent duplicate attacks

        path = attackToPathType[nextAttack];
        path.attackRadius = nextAttack.attackRange;
        path.enabled = true;

        coroutineRunning = false;
    }

    private void ChargeAttack()
    {
        attackRoutine = StartCoroutine(ChargeRoutine());
    }

    public void SlamAttack()
    {
        StartCoroutine(turnTowardsPlayerRoutine(nextAttack.windUp, callback: () => attackRoutine = StartCoroutine(SlamRoutine())));
    }

    public void ShockwaveAttack()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance < nextAttack.attackRange * (1 + 0.25f * shockwaveCount))
        {
            player.GetComponent<EntityHealth>().Damage(logic.baseDamage * nextAttack.damageMultiplier);
            Movement movement = player.GetComponent<Movement>();
            movement.ApplyExternalVelocity((player.transform.position - transform.position).normalized * Mathf.Lerp(20f, 40f, distance/nextAttack.attackRange));
            float jumpSpeed = movement.jumpSpeed;
            movement.jumpSpeed = 15f;
            movement.Jump();
            movement.jumpSpeed = jumpSpeed;
        }

        if(isPhase2)
        {
            if(shockwaveCount == 0)
            {
                hurtScript = Instantiate(hurtCirclePrefab, transform.position, Quaternion.identity).GetComponent<HurtCircle>();
                hurtScript.Initialize("Player", 1, 10, radius: nextAttack.attackRange, fadeInDuration: 1);
                hurtScript.canDamage = false;
            }
            else if(shockwaveCount == 2)
            {
                hurtScript.canDamage = true;
            }
        }

        ++shockwaveCount;
    }

    public void ShootMissile()
    {
        StartCoroutine(turnTowardsPlayerRoutine(nextAttack.windDown, variance: 0.25f, callback: () => 
        {
            for(int i = 0; i < 3; ++i)
            {
                Vector3 towardsPlayer = transform.forward;
                towardsPlayer += new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f));
                towardsPlayer.y = 0;
                GameObject missile = Instantiate(missilePrefab, transform.position + transform.up + transform.forward*4, Quaternion.FromToRotation(transform.position, player.transform.position));
                missile.transform.localScale *= 2;
                missile.GetComponent<Projectile>().Initialize("Player", logic.baseDamage * nextAttack.damageMultiplier, towardsPlayer);
            }
        }));
    }

    private IEnumerator ChargeRoutine()
    {
        path.enabled = false;
        path.agent.enabled = false;

        int numCharges = isPhase2 ? 3 : 1;
        for(int i = 0; i < numCharges; ++i)
        {
            bool flag = true;
            float duration = i == 0 ? nextAttack.windUp : nextAttack.windDown;
            StartCoroutine(turnTowardsPlayerRoutine(duration, 0.25f, () => flag = false));
            while(flag)
            {
                yield return null;
            }

            Vector3 direction = transform.forward;
            direction.y = 0;

            float speed = stats.getMoveSpeed() * 2;

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

            animator.SetTrigger("ChargeStun");
        }

        animator.SetBool("isCharge", false);
        yield return new WaitForSeconds(nextAttack.windDown);

        path.agent.enabled = true;
        path.enabled = true;
        SetCooldown();
    }

    private IEnumerator SlamRoutine()
    {
        path.enabled = false;

        if(isPhase2)
        {
            hurtScript = Instantiate(hurtCirclePrefab, transform.position, Quaternion.identity).GetComponent<HurtCircle>();
            hurtScript.Initialize("Player", 1, 10, radius: 5, fadeInDuration: 1);
            hurtScript.canDamage = false;
        }

        int count = 0;
        while(count < nextAttack.duration/slamDebrisFrequency)
        {
            GameObject debris = Instantiate(debrisPrefab, transform.position, Quaternion.identity);
            debris.GetComponent<FallingDebris>().damage = logic.baseDamage * nextAttack.damageMultiplier;
            ++count;
            yield return new WaitForSeconds(slamDebrisFrequency);
        }
        
        if(isPhase2)
        {
            hurtScript.canDamage = true;
        }

        path.enabled = true;
    }

    private IEnumerator turnTowardsPlayerRoutine(float duration, float variance = 0, CoroutineCallback callback = null)
    {
        float lookProgress = 0;
        Quaternion startingRotation = transform.rotation;

        Vector3 towardsPlayer = player.transform.position - transform.position;
        float randomMagnitude = (towardsPlayer.magnitude * variance)/2;
        towardsPlayer += new Vector3(Random.Range(-randomMagnitude, randomMagnitude), 0, Random.Range(-randomMagnitude, randomMagnitude));
        towardsPlayer.y = 0;
        towardsPlayer = towardsPlayer.normalized;

        while(lookProgress < 1)
        {
            lookProgress += Time.deltaTime/duration;
            transform.rotation = Quaternion.Lerp(startingRotation, Quaternion.LookRotation(towardsPlayer), lookProgress);
            yield return null;
        }

        if(callback != null)
            callback();
    }

    private void MoveToPhase2()
    {
        logic = phase2logic;
        isPhase2 = true;
        animator.SetBool("isPhase2", true);

        attackToPathType.Clear();
        attackToPathType.Add(logic.attacks[0], rangedPath);
        attackToPathType.Add(logic.attacks[1], meleePath);
        attackToPathType.Add(logic.attacks[2], rangedPath);
        attackToPathType.Add(logic.attacks[3], rangedPath);
        
        attackToAnimationTrigger.Clear();
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
