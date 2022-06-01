using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmOfTheRam : Head
{
    private const float baseChargeSpeed = 25f;
    private float chargeSpeed { get { return baseChargeSpeed * Player.instance.stats.getMoveSpeed(); } }

    private Movement movement;
    private Vector3 direction;
    private Coroutine routine;
    private CharacterController controller;

    protected override void Start()
    {
        base.Start();

        movement = player.GetComponentInChildren<Movement>();
        controller = player.GetComponentInChildren<CharacterController>();
    }

    protected override void ActivateHelmet()
    {
        base.ActivateHelmet();
        
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
        
        StopCoroutine(durationRoutine);
        anim.animator.SetTrigger("Duration" + slot.ToString());
        EndCharge();
    }

    private void EndCharge()
    {
        movement.isAttacking = false;
        InputManager.instance.preventInputOverride = false;
        movement.animator.SetBool("IsRunning", false);
    }
}
