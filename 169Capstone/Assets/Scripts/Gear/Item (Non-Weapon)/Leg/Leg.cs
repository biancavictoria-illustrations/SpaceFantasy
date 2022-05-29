using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Leg : NonWeaponItem
{
    void Awake()
    {
        slot = InventoryItemSlot.Legs;
    }

    // START & UPDATE both implemented in children -> if we put anything here, it would get overriden unless we call base.Start() / base.Update()

    // private void Update()
    // {
    //     if(InputManager.instance.useLegs)
    //     {
    //         fire = true;
    //     }
    // }

    // moving this stuff to NonWeaponItem.cs
    
    // public IEnumerator CoolDown()
    // {
    //     InGameUIManager.instance.StartCooldownForItem(InventoryItemSlot.Legs, itemData.CoolDown());
    //     yield return new WaitForSeconds(itemData.CoolDown());
    //     anim.animator.SetTrigger("CooldownLegs");
    // }

    // public IEnumerator Duration()
    // {
    //     InGameUIManager.instance.SetItemIconColor(InventoryItemSlot.Legs, InGameUIManager.SLIME_GREEN_COLOR);
    //     yield return new WaitForSeconds(itemData.Duration());
    //     anim.animator.SetTrigger("DurationLegs");
    // }
}
