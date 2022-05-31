using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Accessories : NonWeaponItem
{
    protected override void Awake()
    {
        base.Awake();
        slot = InventoryItemSlot.Accessory;
    }

    protected virtual void Start()
    {
        InputManager.instance.useAccessory.AddListener(TriggerAbility);

        anim.startAccessory.AddListener(ActivateAccessory);
        anim.endAccessory.AddListener(ResetItemAndTriggerCooldown);
    }

    protected virtual void ActivateAccessory()
    {
        StartDurationRoutine();
    }
}
