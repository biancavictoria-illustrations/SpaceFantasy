using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longsword : Equipment
{
    private bool isAttacking;
    private bool holdingAttack;
    private int heldEffectCounter = 0;
    private int maxHeldEffect = 2;
    private float[] damageModifier = new float[] { 0.75f, 1, 1.25f };

    private int bonusStackCounter = 0;
    private int bonusStackMax = 3;
    private float bonusDuration = 3;
    private float attackSpeedModifierBonus = 0.2f;

    private Player player;
    private Movement movement;
    private AnimationStateController playerAnim;
    private Collider swordCollider;
    private Coroutine attackSpeedRoutine;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        movement = player.GetComponentInChildren<Movement>();

        playerAnim = player.GetComponentInChildren<AnimationStateController>();
        playerAnim.endAttack.AddListener(disableAttacking);

        itemModel.GetComponentInChildren<LongswordCollisionWatcher>().hitEvent.AddListener(DealDamage);
        swordCollider = itemModel.GetComponentInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.isAttacking && !isAttacking)
        {
            isAttacking = true;
            holdingAttack = true;
            movement.isAttacking = true;
        }

        if(holdingAttack && !InputManager.instance.isAttacking)
        {
            holdingAttack = false;
            heldEffectCounter = 0;
        }

        playerAnim.animator.SetBool("IsHoldingAttack", heldEffectCounter > 0);
        playerAnim.animator.SetBool("IsAttacking", isAttacking);
        playerAnim.animator.SetFloat("AttackSpeed", player.stats.getAttackSpeed());
        swordCollider.enabled = playerAnim.hitboxActive;
    }

    public void DealDamage(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Damage");
            bool killed = other.GetComponent<EntityHealth>().Damage(damageModifier[heldEffectCounter] * player.currentStr);
            if(killed)
            {
                if(bonusStackCounter < bonusStackMax)
                    ++bonusStackCounter;

                if(attackSpeedRoutine != null)
                    StopCoroutine(attackSpeedRoutine);
                
                player.stats.SetBonusForStat(this, EntityStats.StatType.AttackSpeed, EntityStats.BonusType.multiplier, bonusStackCounter * attackSpeedModifierBonus);
                attackSpeedRoutine = StartCoroutine(bonusDecayRoutine());
            }

            GameManager.instance.EnableHitStop();
        }
        else
        {
            other.GetComponent<PropJumpBreak>().BreakProp();
        }
    }

    public void disableAttacking()
    {
        if(holdingAttack)
        {
            if(heldEffectCounter < maxHeldEffect)
            {
                ++heldEffectCounter;
            }
            else
            {
                heldEffectCounter = 0;
            }
        }
        else
        {
            isAttacking = false;
            movement.isAttacking = false;
        }
    }

    private IEnumerator bonusDecayRoutine()
    {
        yield return new WaitForSeconds(bonusDuration);
        
        --bonusStackCounter;
        player.stats.SetBonusForStat(this, EntityStats.StatType.AttackSpeed, EntityStats.BonusType.multiplier, bonusStackCounter * attackSpeedModifierBonus);

        if(bonusStackCounter > 0)
            attackSpeedRoutine = StartCoroutine(bonusDecayRoutine());
    }
}
