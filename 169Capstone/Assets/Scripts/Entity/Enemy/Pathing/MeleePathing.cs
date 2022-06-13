using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePathing : Pathing
{
    protected override void HandleMovement()
    {
        if(player == null)
            player = Player.instance.transform;
            
        float distance = Vector3.Distance(player.position, transform.position);

        if(InAttackRange() || !canMove || agent.remainingDistance > 2 * provokedRadius)
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
                sprite.flipX = Mathf.Sign(Vector3.Dot(agent.velocity, Camera.main.transform.right)) > 0;
        }
    }

    public override bool Provoked()
    {
        return !agent.isStopped;
    }

    public override bool InAttackRange()
    {
        if(player == null)
            player = Player.instance.transform;

        return Vector3.Distance(player.position, transform.position) <= attackRadius;
    }
}
