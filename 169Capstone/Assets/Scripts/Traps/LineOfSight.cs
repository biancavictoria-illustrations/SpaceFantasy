using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public TargetPlayer targetPlayer;
    private bool turnVal;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //turnVal = targetPlayer.turn;
            targetPlayer.turn = false;
            if(!targetPlayer.shootPlayer)
                StartCoroutine(DelayFire());
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            targetPlayer.shootPlayer = false;
            Debug.Log(targetPlayer.turn);
            //targetPlayer.turn = true;
        }

    }

    private IEnumerator DelayFire()
    {
        //Debug.Log(targetPlayer.turn);
        yield return new WaitForSeconds(targetPlayer.delayRate);

        //targetPlayer.turn = turnVal;
        targetPlayer.shootPlayer = true;
        targetPlayer.turn = true;
        targetPlayer.timer = targetPlayer.shootRate;
        targetPlayer.ShootProjectile();
    }
}
