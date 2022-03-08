using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longsword : Equipment
{
    private const float meleeRange = 4;
    // private string title = "Berserker's Zweihander";
    private float[] damageModifier = new float[] { 0.75f, 1, 1.25f };
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
    private Movement movement;
    private AnimationStateController playerAnim;
    private InputManager input;
    private Collider swordCollider;
    private bool isAttacking;
    private bool holdingAttack;
    [SerializeField] private GameObject timerPrefab;

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
            bonusStackCounter = 0;
            player.stats.SetBonusForStat(this, EntityStats.StatType.AttackSpeed, EntityStats.BonusType.multiplier, 0);
        }

        playerAnim.animator.SetBool("IsAttacking", isAttacking);
        playerAnim.animator.SetFloat("AttackSpeed", player.stats.getAttackSpeed());
        swordCollider.enabled = playerAnim.hitboxActive;
    }

    public void DealDamage(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Damage");
            other.GetComponent<EntityHealth>().Damage(damageModifier[heldEffectCounter] * player.currentStr);
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
            if(bonusStackCounter < bonusStackMax)
                ++bonusStackCounter;
        }
        else
        {
            isAttacking = false;
            movement.isAttacking = false;
        }

        player.stats.SetBonusForStat(this, EntityStats.StatType.AttackSpeed, EntityStats.BonusType.multiplier, bonusStackCounter * attackSpeedModifierBonus);
    }
}
