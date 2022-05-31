using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrousersOfFortitude : Leg
{
    // Doubles Defense for the duration. reduces movement speed by 25%.

    public float slowDownValue = 0.25f; // Percent to slow movement speed by when activated

    protected override void ActivateLegs()
    {
        base.ActivateLegs();

        UpdateStats(1);
    }

    public override void ResetItemAndTriggerCooldown()
    {
        base.ResetItemAndTriggerCooldown();
        
        UpdateStats(0);
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
