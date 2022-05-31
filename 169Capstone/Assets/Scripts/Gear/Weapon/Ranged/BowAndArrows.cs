using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAndArrows : Equipment
{
    private const float minDamagePercent = 0.25f;
    private const float maxDamagePercent = 1f;
    private const float maxHoldTime = 0.5f;
    private const float superChargeDuration = 0.5f;

    private bool isAttacking;
    private bool holdingAttack;
    private float heldTime;

    private Player player;
    private Movement movement;
    private AnimationStateController playerAnim;
    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private GameObject arrowUIPrefab;
    private ArrowUI arrowUI;

    void Start()
    {
        player = Player.instance;
        movement = player.GetComponentInChildren<Movement>();

        playerAnim = player.GetComponentInChildren<AnimationStateController>();
        playerAnim.endAttack.AddListener(disableAttacking);

        arrowUI = Instantiate(arrowUIPrefab, player.GetComponentInChildren<Canvas>().transform).GetComponent<ArrowUI>();
        arrowUI.maximum = maxHoldTime;
        arrowUI.current = heldTime;
    }

    void Update()
    {
        arrowUI.current = heldTime;
        if(InputManager.instance.isAttacking && !isAttacking)
        {
            isAttacking = true;
            holdingAttack = true;
            movement.isAttacking = true;
            playerAnim.animator.SetBool("IsBowAttacking", true);
        }

        if(playerAnim.attackActive)
        {
            if(InputManager.instance.isAttacking)
            {
                if(heldTime < maxHoldTime + superChargeDuration)
                    heldTime += Time.deltaTime;

                if(heldTime > maxHoldTime + superChargeDuration)
                    heldTime = maxHoldTime + superChargeDuration;
            }
            else if(holdingAttack)
            {
                FireArrow();
            }
        }
    }

    private void FireArrow()
    {
        holdingAttack = false;

        GameObject arrow = Instantiate(arrowPrefab, player.transform.position + Vector3.up*2, player.transform.rotation);
        Projectile projectileScript = arrow.GetComponent<Projectile>();
        if(!projectileScript)
        {
            Destroy(arrow);
            Debug.LogError("Projectile prefab " + arrowPrefab + " did not contain a Projectile script.");
        }

        float damageMultiplier;
        if(heldTime > maxHoldTime && heldTime < maxHoldTime + superChargeDuration)
            damageMultiplier = 2;
        else
            damageMultiplier = Mathf.Lerp(minDamagePercent, maxDamagePercent, heldTime/maxHoldTime);
        
        projectileScript.Initialize(LayerMask.NameToLayer("Enemy"), player.stats.getDEXDamage() * damageMultiplier, DamageSourceType.Player, InputManager.instance.cursorLookDirection, speed: 20 * Mathf.Lerp(1, 2, heldTime/maxHoldTime));

        playerAnim.animator.SetBool("IsBowAttacking", false);
        heldTime = 0;
    }

    public void disableAttacking()
    {
        isAttacking = false;
        movement.isAttacking = false;
    }

    public override void ManageCoroutinesOnUnequip()
    {
        // No coroutines to deal with (I think?)
        // Can leave this blank
    }
}
