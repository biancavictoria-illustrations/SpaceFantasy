using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleAnimatorController : MonoBehaviour
{
    public enum AttackType
    {
        Charge,
        Slam,
        Shockwave,
        ArcaneMissiles
    }

    public Beetle beetle;

    public void SetCooldown()
    {
        beetle.SetCooldown();
    }

    public void PerformAttack(AttackType attack)
    {
        switch(attack)
        {
            case AttackType.Charge:
                break;

            case AttackType.Slam:
                beetle.SlamAttack();
                break;

            case AttackType.Shockwave:
                beetle.ShockwaveAttack();
                break;

            case AttackType.ArcaneMissiles:
                break;

            default:
                break;
        }
    }
}
