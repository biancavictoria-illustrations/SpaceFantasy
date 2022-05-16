using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrousersOfFortitude : Leg
{
    // Doubles Defense for the duration. reduces movement speed by 25%.

    private Player player;

    private void Start()
    {
        player = Player.instance;
        anim = player.GetComponentInChildren<AnimationStateController>();
        anim.startLegs.AddListener(ActivateTrousers);
        anim.endLegs.AddListener(ResetTrousers);
        anim.startCooldownLegs.AddListener(StartCooldown);
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
        StartCoroutine("Duration");
    }

    public void ResetTrousers()
    {
        UpdateStats(0);
        clearToFire = true;
    }

    private void UpdateStats(int fullBonus)
    {
        player.stats.SetBonusForStat(this, StatType.Defense, EntityStats.BonusType.flat, player.stats.getDefense() * fullBonus);
        player.stats.SetBonusForStat(this, StatType.MoveSpeed, EntityStats.BonusType.flat, player.stats.getMoveSpeed() * -0.25f * fullBonus);
    }
}
