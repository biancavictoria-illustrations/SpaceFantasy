using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropulsionHeels : Leg
{
    private const float dashBaseSpeed = 40f;

    private Vector3 direction;
    private Coroutine routine;
    private CharacterController controller;

    protected override void Start()
    {
        base.Start();

        controller = player.GetComponent<CharacterController>();
    }

    protected override void ActivateLegs()
    {
        base.ActivateLegs();

        direction = controller.velocity.normalized;
        InputManager.instance.preventInputOverride = true;
        StartCoroutine(dashRoutine());
    }

    public override void ResetItemAndTriggerCooldown()
    {
        base.ResetItemAndTriggerCooldown();
        
        InputManager.instance.preventInputOverride = false;
        StopCoroutine(routine);
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
