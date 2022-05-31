using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Head : NonWeaponItem
{
    protected override void Awake()
    {
        base.Awake();
        slot = InventoryItemSlot.Helmet;
    }

    protected virtual void Start()
    {
        InputManager.instance.useHelmet.AddListener(TriggerAbility);

        anim.startHelmet.AddListener(ActivateHelmet);
        anim.endHelmet.AddListener(ResetItemAndTriggerCooldown);
    }

    protected virtual void ActivateHelmet()
    {
        StartDurationRoutine();
    }
}
