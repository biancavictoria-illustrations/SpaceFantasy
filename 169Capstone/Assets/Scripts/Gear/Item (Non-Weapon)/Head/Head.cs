using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Head : NonWeaponItem
{
    void Awake()
    {
        slot = InventoryItemSlot.Helmet;
    }

    // START & UPDATE both implemented in children -> if we put anything here, it would get overriden unless we call base.Start() / base.Update()

    // private void Update()
    // {
    //     if(InputManager.instance.useHead)
    //     {
    //         fire = true;
    //     }
    // }
}
