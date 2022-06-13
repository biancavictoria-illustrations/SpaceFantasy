using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropulsionHeels : Leg
{
    private const float dashBaseSpeed = 40f;

    private Vector3 direction;
    private Coroutine routine;
    private CharacterController controller;
    private Transform playerModel;

    protected override void Start()
    {
        base.Start();

        controller = player.GetComponent<CharacterController>();
        playerModel = player.GetComponent<Movement>().model;
    }

    protected override void ActivateLegs()
    {
        base.ActivateLegs();

        direction = controller.velocity.normalized;
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        if(direction == Vector3.zero)
            direction = playerModel.forward;
        
        InputManager.instance.preventInputOverride = true;
        routine = StartCoroutine(dashRoutine());
    }

    public override void ResetItemAndTriggerCooldown()
    {
        base.ResetItemAndTriggerCooldown();
        
        InputManager.instance.preventInputOverride = false;
        if(routine != null){
            StopCoroutine(routine);
        }
    }

    public override void ManageCoroutinesOnUnequip()
    {
        base.ManageCoroutinesOnUnequip();

        InputManager.instance.preventInputOverride = false;
        if(routine != null){
            StopCoroutine(routine);
        }
    }

    private IEnumerator dashRoutine()
    {
        while(durationRoutine != null)
        {
            controller.Move(direction * dashBaseSpeed * player.stats.getMoveSpeed() * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }
}
