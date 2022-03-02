using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichAnimatorController : MonoBehaviour
{
    public enum AttackType
    {
        MagicMissile,
        MeteorShower,
        SinisterSummons,
        TimeDilation
    }

    public Lich lich;

    public void SetCooldown()
    {
        lich.SetCooldown();
    }

    public void PerformAttack(AttackType attack)
    {
        switch(attack)
        {
            case AttackType.MagicMissile:
                lich.MagicMissile();
                break;

            case AttackType.MeteorShower:
                lich.MeteorShower();
                break;

            case AttackType.SinisterSummons:
                lich.SinisterSummons();
                break;

            case AttackType.TimeDilation:
                lich.TimeDilation();
                break;

            default:
                break;
        }
    }
}
