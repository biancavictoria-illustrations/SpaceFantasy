using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Leg : NonWeaponItem
{
    protected override void Awake()
    {
        base.Awake();
        slot = InventoryItemSlot.Legs;
    }

    protected virtual void Start()
    {
        InputManager.instance.useLegs.AddListener(TriggerAbility);

        anim.startLegs.AddListener(ActivateLegs);
        anim.endLegs.AddListener(ResetItemAndTriggerCooldown);
    }

    protected virtual void ActivateLegs()
    {
        StartDurationRoutine();
    }
}
