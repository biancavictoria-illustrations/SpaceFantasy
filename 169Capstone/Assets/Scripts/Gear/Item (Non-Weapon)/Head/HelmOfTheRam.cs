using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HelmOfTheRam : Head
{
    private const float baseChargeSpeed = 25f;
    private float chargeSpeed { get { return baseChargeSpeed * Player.instance.stats.getMoveSpeed(); } }

    [SerializeField] private GameObject chargeVFX;

    private Pathing enemyPathing;
    private Movement movement;
    private Vector3 direction;
    private Coroutine routine;
    private Coroutine pushRoutine;
    private CharacterController controller;
    private GameObject chargeVFXInstance;

    protected override void Start()
    {
        base.Start();

        movement = player.GetComponentInChildren<Movement>();
        controller = player.GetComponentInChildren<CharacterController>();
    }

    protected override void ActivateHelmet()
    {
        base.ActivateHelmet();

        Transform model = player.GetComponent<Movement>().model;
        chargeVFXInstance = Instantiate(chargeVFX, player.transform.position, model.rotation, model);
        
        movement.isAttacking = true;
        movement.animator.SetBool("IsRunning", true);

        InputManager.instance.isAttacking = false;
        InputManager.instance.preventInputOverride = true;

        direction = InputManager.instance.cursorLookDirection;
        routine = StartCoroutine(chargeRoutine());
    }

    public override void ResetItemAndTriggerCooldown()
    {
        base.ResetItemAndTriggerCooldown();

        if(routine != null)
        {
            StopCoroutine(routine);
            EndCharge();
        }
    }

    public override void ManageCoroutinesOnUnequip()
    {
        base.ManageCoroutinesOnUnequip();

        if(routine != null)
        {
            StopCoroutine(routine);
            EndCharge();
        }

        if(pushRoutine != null)
        {
            StopCoroutine(pushRoutine);

            if(enemyPathing != null)
            {
                enemyPathing.enabled = true;
                enemyPathing.agent.enabled = true;
            }
        }
    }

    private IEnumerator chargeRoutine()
    {
        RaycastHit hit;

        while(!Physics.SphereCast(Player.instance.transform.position + controller.center, 
                                             controller.radius, 
                                             direction, 
                                             out hit, 
                                             chargeSpeed * Time.fixedDeltaTime,
                                             LayerMask.GetMask("Enemy", "Environment"),
                                             QueryTriggerInteraction.UseGlobal))
        {
            controller.Move(direction * chargeSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EntityHealth enemy = hit.collider.GetComponent<EntityHealth>();
            enemy.Damage(player.stats.getSTRDamage(false) * 2, DamageSourceType.Player);

            Pathing path = enemy.GetComponent<Pathing>();
            if(path != null)
                pushRoutine = StartCoroutine(pushEnemyRoutine(path));
        }
        
        StopCoroutine(durationRoutine);
        durationRoutine = null;

        anim.animator.SetTrigger("Duration" + slot.ToString());
        EndCharge();
    }

    private void EndCharge()
    {
        if(chargeVFXInstance != null)
            Destroy(chargeVFXInstance);

        movement.isAttacking = false;
        InputManager.instance.preventInputOverride = false;
        movement.animator.SetBool("IsRunning", false);
    }

    private IEnumerator pushEnemyRoutine(Pathing enemyPathing)
    {
        float startTime = Time.time;
        this.enemyPathing = enemyPathing;

        enemyPathing.enabled = false;
        enemyPathing.agent.enabled = false;

        while(Time.time < startTime + 0.2f)
        {
            if(enemyPathing == null)
                yield break;

            yield return new WaitForFixedUpdate();
            enemyPathing.transform.Translate(direction.normalized * 15 * Time.fixedDeltaTime, Space.World);
        }

        if(enemyPathing != null)
        {
            enemyPathing.enabled = true;
            enemyPathing.agent.enabled = true;
        }
    }
}
