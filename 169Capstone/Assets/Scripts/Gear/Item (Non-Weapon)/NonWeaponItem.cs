using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonWeaponItem : Equipment
{
    public GameObject timerPrefab;
    public ActivatedItemData itemData;
    public InventoryItemSlot slot {get; protected set;}
    public bool fire = false;
    [HideInInspector] public bool clearToFire = true;

    [HideInInspector] public AnimationStateController anim;
    //[HideInInspector] public Timer timer;

    public Coroutine cooldownRoutine {get; protected set;}
    public Coroutine durationRoutine {get; protected set;}

    // AWAKE, START, & UPDATE implemented in children -> if we put anything here, it would get overriden unless we call base.Start(), etc.

    // Call these in the children to activate the cooldown & duration routines
    protected void StartCooldownRoutine()
    {
        cooldownRoutine = StartCoroutine(CoolDown());
    }

    protected void StartDurationRoutine()
    {
        durationRoutine = StartCoroutine(Duration());
    }

    // Always triggered at the end of the item's Reset function, which is called once Duration is complete
    private IEnumerator CoolDown()
    {
        InGameUIManager.instance.StartCooldownForItem(slot, itemData.CoolDown());
        yield return new WaitForSeconds(itemData.CoolDown());

        // NOTE: For this to work (& the one in Duration), this trigger has to use the item slot exactly as it's spelled in the type
        // (in the animator, make sure to use the InventoryItemSlot values exactly as: "Helmet", "Accessory", "Legs")
        anim.animator.SetTrigger("Cooldown" + slot.ToString());
    }

    private IEnumerator Duration()
    {
        InGameUIManager.instance.SetItemIconColor(slot, InGameUIManager.SLIME_GREEN_COLOR);
        yield return new WaitForSeconds(itemData.Duration());
        anim.animator.SetTrigger("Duration" + slot.ToString());
    }

    // Any children with specific coroutines to deal with also need their own version of this with base.ManageCoroutinesOnUnequip(); at the beginning
    public override void ManageCoroutinesOnUnequip()
    {
        // If we're in the duration, go to cooldown and then end cooldown (but continue the UI)
        // Item SLOT is still on cooldown, can't use this ability yet, but we switched items so need to stop these instances specifically
        if(durationRoutine != null){
            StopCoroutine(durationRoutine);
            anim.animator.SetTrigger("Duration" + slot.ToString());
            anim.animator.SetTrigger("Cooldown" + slot.ToString());
            InGameUIManager.instance.StartCooldownForItem(slot, itemData.CoolDown());
        }

        // If mid-cooldown, just stop the cooldown coroutine (but continue the UI)
        if(cooldownRoutine != null){
            StopCoroutine(cooldownRoutine);
            anim.animator.SetTrigger("Cooldown" + slot.ToString());
        }
    }

    public abstract void ResetItemAndTriggerCooldown();
}
