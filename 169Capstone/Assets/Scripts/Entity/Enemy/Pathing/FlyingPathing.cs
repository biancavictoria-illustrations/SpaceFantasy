using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPathing : Pathing
{
    private bool canPath = true;
    private Vector3 destination;

    protected override void HandleMovement()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if(!canPath || InAttackRange() || gameManager.inShopMode)
        {
            agent.isStopped = true;
            destination = Vector3.zero;
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

            if(destination == Vector3.zero)
            {
                int numTries = 0;
                bool successfulDest = false;
                do
                {
                    float angle = Vector3.Angle(Vector3.forward, transform.position - player.position) + Random.Range(-0.25f, 0.25f) * Mathf.PI;
                    float magnitude = attackRadius - Random.Range(1, 4);

                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0,  Mathf.Sin(angle)) * magnitude;
                    destination = player.position + offset;
                    successfulDest = agent.SetDestination(destination);
                    ++numTries;
                    Debug.Log(offset);
                }
                while(!successfulDest && numTries < 20);

                if(!successfulDest)
                {
                    StartCoroutine(DisablePathing(0.5f));
                    destination = Vector3.zero;
                }
            }

            if(agent.remainingDistance < 0.5f)
            {
                destination = Vector3.zero;
                Debug.Log("Choose new destination");
            }
        }
        
        //Flip the sprite to face the player
        sprite.transform.localScale = new Vector3(-Mathf.Sign(Vector3.Dot(player.position - transform.position, Camera.main.transform.right)), sprite.transform.localScale.y, sprite.transform.localScale.z);
    }

    public override bool Provoked()
    {
        return !agent.isStopped;
    }

    public override bool InAttackRange()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        //If still moving toward the destination point
        if(agent.remainingDistance > 0.5f)
            return false;

        //If too close or too far away return false
        if(distance > attackRadius || distance < 3)
            return false;

        Debug.Log("Is in range");

        //If enemy doesn't have line of sight return false
        if(Physics.Raycast(transform.position + Vector3.up * 2, player.position - transform.position, distance, LayerMask.GetMask("Environment")))
            return false;

        Debug.Log("Has LOS");

        return true;
    }

    private IEnumerator DisablePathing(float seconds)
    {
        canPath = false;
        yield return new WaitForSeconds(seconds);
        canPath = true;
    }
}
