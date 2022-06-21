using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTrap : PropJumpBreak
{
    public TargetPlayer targetPlayer;
    public LineOfSight lineOfSight;
    [SerializeField] private GameObject fX;
     [SerializeField] private GameObject fX2;

    protected override void OnTriggerEnter(Collider other)
    {
    }

    public override void BreakProp()
    {
        targetPlayer.enabled = false;

        lineOfSight.enabled = false;
        if(lineOfSight.delayFireCoroutine != null)
        {
            StopCoroutine(lineOfSight.delayFireCoroutine);
        }
        

        if(fX != null)
        {
            fX.SetActive(false);
            
        }
        if(fX2 != null)
        {
            fX2.SetActive(false);
            
        }
    }
}
