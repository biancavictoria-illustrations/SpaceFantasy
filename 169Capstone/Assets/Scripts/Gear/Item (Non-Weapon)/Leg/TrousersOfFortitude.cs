using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrousersOfFortitude : Leg
{
    // Doubles Defense for the duration. reduces movement speed by 25%.

    private Player player;

    public float slowDownValue = 0.25f; // Percent to slow movement speed by when activated

    private void Start()
    {
        player = Player.instance;
        anim = player.GetComponentInChildren<AnimationStateController>();
        anim.startLegs.AddListener(ActivateTrousers);
        anim.endLegs.AddListener(ResetItemAndTriggerCooldown);
    }

    private void Update()
    {
        if(InputManager.instance.useLegs && clearToFire)
        {
            clearToFire = false;
            anim.animator.SetBool("IsUseLegs", true);
            InputManager.instance.useLegs = false;
        }
    }

    public void ActivateTrousers()
    {
        anim.animator.SetBool("IsUseLegs", false);
        UpdateStats(1);
        StartDurationRoutine();
    }

    public override void ResetItemAndTriggerCooldown()
    {
        UpdateStats(0);
        clearToFire = true;
        StartCooldownRoutine();
    }

    private void UpdateStats(int fullBonus)
    {
        player.stats.SetBonusForStat(this, StatType.Defense, EntityStats.BonusType.multiplier, fullBonus);
        player.stats.SetBonusForStat(this, StatType.MoveSpeed, EntityStats.BonusType.multiplier, -slowDownValue * fullBonus);
    }

    public override void ManageCoroutinesOnUnequip()
    {
        base.ManageCoroutinesOnUnequip();

        // Revert stat changes from activated ability
        UpdateStats(0);
    }
}
