using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmOfTheRam : Head
{
    private Collider hitCollider;

    protected override void ActivateHelmet()
    {
        base.ActivateHelmet();
        
        // make the player do the charge thing
    }

    private bool DetectCollision()
    {
        RaycastHit hit;

        if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, 3))
        {
            hitCollider = hit.collider;
            return true;
        }
        else
        {
            return false;
        }
    }
}
