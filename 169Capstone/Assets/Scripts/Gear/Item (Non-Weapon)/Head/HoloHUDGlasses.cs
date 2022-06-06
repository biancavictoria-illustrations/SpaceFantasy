using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloHUDGlasses : Head
{
    protected override void ActivateHelmet()
    {
        base.ActivateHelmet();
        UpdateStats(1,1);
    }

    public override void ResetItemAndTriggerCooldown()
    {
        base.ResetItemAndTriggerCooldown();
        UpdateStats(0,0);
    }

    private void UpdateStats(float critBonus, float dodgeBonus)
    {
        player.stats.SetBonusForStat(this, StatType.CritChance, EntityStats.BonusType.multiplier, critBonus);
        player.stats.SetBonusForStat(this, StatType.DodgeChance, EntityStats.BonusType.multiplier, dodgeBonus);
    }

    public override void ManageCoroutinesOnUnequip()
    {
        base.ManageCoroutinesOnUnequip();

        // Revert stat changes from activated ability
        UpdateStats(0,0);
    }
}
