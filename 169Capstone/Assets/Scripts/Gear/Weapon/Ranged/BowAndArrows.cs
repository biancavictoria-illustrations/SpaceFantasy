using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAndArrows : Equipment
{
    // private string title = "Bow And Arrows";
    private float minDamageModifier = 0.25f;
    private float maxDamageModifier = 1;
    //private float damageIncrement = 0.25f;
    private float maxAttackSpeedModifier = 2;
    //private float minAttackSpeedModifier = 1f;
    //private float attackSpeedIncrement = 0.25f;
    private float heldCounter = 0.25f;
    private bool superCharged = false;
    private float superChargedDuration = 0.5f;
    //private float superChargeDamageModifier = 1;
    private bool currentlyAttacking = false;
    private float windUp = 0.1f;
    private float minDuration = 0;
    //private float maxDuration = 0.5f;
    private float windDown = 0.4f;
    [SerializeField] private float range = 15;
    private float currentCharge = 0;
    private float bonusDamage = 0;

    private Player player;
    [SerializeField] private GameObject timerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.isAttacking && !currentlyAttacking)
        {
            currentlyAttacking = true;
            StartCoroutine(PrimaryAttack());
            // StartCoroutine(CallDamage());
        }
    }

    private IEnumerator PrimaryAttack()
    {
        float charge = 0;
        int superChargedCall = 0;

        while(InputManager.instance.isAttacking)
        {
            //Debug.Log(minDamageModifier + (charge * heldCounter));
            if(minDamageModifier + (charge * heldCounter) == maxDamageModifier && superChargedCall == 0)
            {
                superChargedCall++;
                //Debug.Log("Super Charged");
                /*superCharged = true;
                StartCoroutine(DamageBonus(Instantiate(timerPrefab).GetComponent<Timer>()));*/
                SecondaryAbility();
            }
            else if(minDamageModifier + (charge * heldCounter) < maxDamageModifier)
            {
                charge++;
            }

            yield return null;
        }

        //float attackSpeed = player.currentAttackSpeed + maxAttackSpeedModifier - (charge * heldCounter);
        float duration = minDuration + (charge * heldCounter);
        currentCharge = charge;

        // StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, windUp * attackSpeed, duration * attackSpeed, windDown * attackSpeed));

        // yield return new WaitUntil(() => baseAttack.Completed);
        Debug.Log("Out of Attack loop");
        currentlyAttacking = false;
    }

    // private IEnumerator CallDamage()
    // {
    //     /*baseAttack.damageDealt = true;
    //     yield return null;*/
    //     yield return new WaitUntil(() => baseAttack.hit);
    //     baseAttack.enemyDeath = DealDamage();
    //     baseAttack.damageDealt = true;
    // }

    private bool DealDamage()
    {
        RaycastHit hit;
        float damageModifier = minDamageModifier + (currentCharge * heldCounter) + bonusDamage;
        Debug.Log("Actually Hitting");

        if(Physics.Raycast(player.GetComponent<Transform>().position, player.GetComponent<Transform>().forward, out hit, range))
        {
            Debug.Log("hit " + hit.collider.name);
            return hit.collider.tag == "Enemy" ? hit.collider.GetComponent<EntityHealth>().Damage(player.currentDex * damageModifier) : false;
        }
        else
        {
            Debug.Log("miss");
            return false;
        }
    }

    private IEnumerator DamageBonus(Timer timer)
    {
        bonusDamage = player.currentDex;
        timer.StartTimer(superChargedDuration);
        Debug.Log("Start of bonus");
        yield return new WaitUntil(() => timer.timeRemaining <= 0);

        if(!InputManager.instance.isAttacking)
        {
            // yield return new WaitUntil(() => baseAttack.Completed);
        }

        bonusDamage = 0;
        Debug.Log("End of bonus");
        superCharged = false;
        Destroy(timer.gameObject);
    }

    private void SecondaryAbility()
    {
        if(!superCharged)
        {
            superCharged = true;
            StartCoroutine(DamageBonus(Instantiate(timerPrefab).GetComponent<Timer>()));
        }
    }
}
