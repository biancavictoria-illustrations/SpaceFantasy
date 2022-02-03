using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Longsword : MonoBehaviour
{
    private const float meleeRange = 4;

    private string title = "Berserker's Zweihander";
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
    private AnimationStateController playerAnim;
    private InputManager input;
    [SerializeField] private GameObject timerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerAnim = player.GetComponentInChildren<AnimationStateController>();
        StartCoroutine(WatchForAttack());
    }

    // Update is called once per frame
    void Update()
    {
        playerAnim.animator.SetBool("IsAttacking", InputManager.instance.isAttacking);
    }

    public bool DealDamage()
    {
        Debug.DrawRay(player.transform.position + 2*Vector3.up, InputManager.instance.cursorLookDirection * meleeRange, Color.yellow, 1);
        RaycastHit hit;
        if(Physics.SphereCast(player.transform.position + 2*Vector3.up, 1, InputManager.instance.cursorLookDirection, out hit, meleeRange, LayerMask.GetMask("Enemy")))
        {
            Debug.Log("Damage");
            return hit.collider.GetComponent<EntityHealth>().Damage(damageModifier[heldEffectCounter] * player.currentStr);
        }
        else
        {
            Debug.Log("No Damage");
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

    private IEnumerator WatchForAttack()
    {
        while(true)
        {
            yield return null;
            if(playerAnim.attackActive)
            {
                playerAnim.attackActive = false;
                DealDamage();
            }
        }
    }
}
