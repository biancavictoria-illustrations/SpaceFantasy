using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public TargetPlayer targetPlayer;
    private bool turnVal;
    private bool turretActive = false;
    public Coroutine delayFireCoroutine;

    private void Start()
    {        
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //turnVal = targetPlayer.turn;
            targetPlayer.turn = false;
            turretActive = true;
            if(!targetPlayer.shootPlayer)
                delayFireCoroutine = StartCoroutine(DelayFire());
        }            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            targetPlayer.shootPlayer = false;
            turretActive = false;
            // Debug.Log(targetPlayer.turn);
            // targetPlayer.turn = true;
        }
    }

    private IEnumerator DelayFire()
    {
        //Debug.Log(targetPlayer.turn);
        yield return new WaitForSeconds(targetPlayer.delayRate);
        
        targetPlayer.turn = true;

        if (turretActive == false)
        {
            yield break;
        }
        
        //targetPlayer.turn = turnVal;
        targetPlayer.shootPlayer = true;
        
        targetPlayer.timer = targetPlayer.shootRate;
        // targetPlayer.ShootProjectile();
    }
}
