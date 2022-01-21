using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longsword : MonoBehaviour
{
    private string title = "Berserker's Zweihander";
    private float[] damageModifier = new float[] { 0.75f, 1, 1.25f };
    private float meleeRange = 3;
    private float range;
    [SerializeField] private float rangeModifier = 0.1f; 
    private int heldEffectCounter = 0;
    private int maxHeldEffect = 3;
    private float[] windUp = new float[] { 0.25f, 0.5f, 0.5f };
    private float heldDuration = 0.25f;
    private float windDown = 0.75f;
    private float secondaryDuration = 3;
    private float attackSpeedModifierBonus = 0.2f;
    private int bonusStackCounter = 0;
    private int bonusStackMax = 3;
    private bool currentlyAttacking = false;

    private Player player;
    private AnimationStateController playerAnim;
    private EntityAttack baseAttack;
    [SerializeField] private GameObject timerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerAnim = player.GetComponentInChildren<AnimationStateController>();
        baseAttack = player.GetComponent<EntityAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.isAttacking && !currentlyAttacking)
        {
            currentlyAttacking = true;
            StartCoroutine(PrimaryAttack());
            StartCoroutine(CallDamage());
        }
    }

    private IEnumerator PrimaryAttack()
    {
        //Debug.Log("Called");
        while(heldEffectCounter < maxHeldEffect)
        {
            range = meleeRange + (rangeModifier * heldEffectCounter * meleeRange);
            StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, windUp[heldEffectCounter] * player.currentAttackSpeed, heldDuration * player.currentAttackSpeed));
            playerAnim.ToggleAttackAnimation(true);
            yield return new WaitUntil(() => baseAttack.Completed);

            heldEffectCounter++;

            if(!InputManager.instance.isAttacking)
            {
                break;
            }
        }

        StartCoroutine(baseAttack.WindDown(Instantiate(timerPrefab).GetComponent<Timer>(), windDown * player.currentAttackSpeed));
        yield return new WaitUntil(() => baseAttack.Completed);

        heldEffectCounter = 0;
        currentlyAttacking = false;
    }

    private IEnumerator CallDamage()
    {
        yield return new WaitUntil(() => baseAttack.hit);
        baseAttack.enemyDeath = DealDamage();
        if(baseAttack.enemyDeath)
        {
            SecondaryAbility();
        }
        baseAttack.damageDealt = true;
    }

    private bool DealDamage()
    {
        RaycastHit hit;
        if(Physics.SphereCast(player.GetComponent<Transform>().position, 3, player.GetComponent<Transform>().forward, out hit, range))
        {
            return hit.collider.tag == "Enemy" ? hit.collider.GetComponent<EntityHealth>().Damage(damageModifier[heldEffectCounter] * player.currentStr) : false;
        }
        else
        {
            return false;
        }

    }

    private void SecondaryAbility()
    {
        if(bonusStackCounter < bonusStackMax)
        {
            bonusStackCounter++;
            StartCoroutine(SpeedBonus(Instantiate(timerPrefab).GetComponent<Timer>()));
        }
    }

    private IEnumerator SpeedBonus(Timer timer)
    {
        float bonus = player.currentAttackSpeed * attackSpeedModifierBonus;
        player.currentAttackSpeed += bonus;
        timer.StartTimer(secondaryDuration);
        yield return new WaitUntil(() => timer.timeRemaining <= 0);
        player.currentAttackSpeed -= bonus;
        bonusStackCounter--;
        Destroy(timer.gameObject);
    }
}
