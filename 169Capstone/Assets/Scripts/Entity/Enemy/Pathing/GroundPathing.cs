using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPathing : Pathing
{
    protected override void HandleMovement()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if(InAttackRange() || gameManager.inShopMode || agent.remainingDistance > 2 * provokedRadius)
        {
            agent.isStopped = true;
        }
        else if(distance <= provokedRadius && !attacking)
        {
            agent.isStopped = false;
        }

        if(agent.remainingDistance > 2 * provokedRadius)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if(!agent.isStopped)
        {
            agent.speed = speed;
            agent.SetDestination(player.position);

            //Flip the sprite to face the direction it's moving
            if(sprite)
                sprite.transform.localScale = new Vector3(-Mathf.Sign(Vector3.Dot(agent.velocity, Camera.main.transform.right)), sprite.transform.localScale.y, sprite.transform.localScale.z);
        }
    }

    public override bool Provoked()
    {
        return !agent.isStopped;
    }

    public override bool InAttackRange()
    {
        return Vector3.Distance(player.position, transform.position) <= attackRadius;
    }
}
