using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Accessories : NonWeaponItem
{
    void Awake()
    {
        slot = InventoryItemSlot.Accessory;
    }

    // START & UPDATE both implemented in children -> if we put anything here, it would get overriden unless we call base.Start() / base.Update()

    // private void Update()
    // {
    //     if(InputManager.instance.useAccessory)
    //     {
    //         Debug.Log("fire");
    //         fire = true;
    //     }
    // }

    // Moving this to NonWeaponItem

    // public IEnumerator CoolDown()
    // {
    //     InGameUIManager.instance.StartCooldownForItem(InventoryItemSlot.Accessory, itemData.CoolDown());
    //     yield return new WaitForSeconds(itemData.CoolDown());
    //     anim.animator.SetTrigger("CooldownAccessory");
    // }
}
