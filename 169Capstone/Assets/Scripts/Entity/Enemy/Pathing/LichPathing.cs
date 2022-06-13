using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LichPathing : Pathing
{
    public float nearAttackRange = 7;
    public bool needsLOS = true;

    private bool canPath = true;
    private Vector3 destination;
    private NavMeshPath path;

    void Awake()
    {
        path = new NavMeshPath();
    }

    protected override void HandleMovement()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if(!canPath || InAttackRange() || !canMove)
        {
            //Debug.Log("Entered Attack Range");
            agent.isStopped = true;
            destination = Vector3.zero;
        }
        else if(distance <= provokedRadius && !attacking)
        {
            agent.isStopped = false;
        }
        else if(distance > 2 * provokedRadius)
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
                Dictionary<Vector3, float> positionsWithWeights = new Dictionary<Vector3, float>();
                bool successfulDest = false;
                do
                {
                    //Set the distance to be generally within the attack range
                    float magnitude = Random.Range(nearAttackRange + 1, attackRadius - 1);
                    float angle = Random.Range(0, 2 * Mathf.PI);

                    //Set the destination to a point that is a combination of the above angle and magnitude relative to the player
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0,  Mathf.Sin(angle)) * magnitude;
                    destination = player.position + offset;
                    successfulDest = agent.CalculatePath(destination, path);
                    if(successfulDest)
                    {
                        NavMeshHit hit = new NavMeshHit();
                        NavMesh.SamplePosition(destination, out hit, 1, NavMesh.AllAreas);
                        if(hit.mask == NavMesh.GetAreaFromName("Unfavorable"))
                        {
                            angle = Random.Range(0, 2 * Mathf.PI);
                            offset = new Vector3(Mathf.Cos(angle), 0,  Mathf.Sin(angle)) * magnitude;
                            destination = player.position + offset;
                            successfulDest = agent.CalculatePath(destination, path);
                        }
                    }
                    ++numTries;
                }
                while(!successfulDest && numTries < 20); //Try to find a valid point 20 times before giving up

                if (successfulDest) //If we couldn't find a valid point in 20 attempts, wait 0.5 seconds before trying again
                {
                    agent.Warp(destination);
                }
                else
                {
                    StartCoroutine(DisablePathing(0.5f));
                    destination = Vector3.zero;
                }
            }

            if(agent.remainingDistance < 0.5f) //If we haven't entered the valid attack range by the time we reach our destination, try again
            {
                destination = Vector3.zero;
                Debug.Log("Choose new destination");
            }
        }
        
        //Flip the sprite to face the player
        if(sprite)
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
        if(distance > attackRadius || distance < nearAttackRange)
            return false;

        //If enemy doesn't have line of sight return false
        if(needsLOS && Physics.Raycast(transform.position + Vector3.up * 2, player.position - transform.position, distance, LayerMask.GetMask("Environment")))
            return false;

        return true;
    }

    private IEnumerator DisablePathing(float seconds)
    {
        Debug.Log("Couldn't find valid destination, disabling pathing");
        canPath = false;
        yield return new WaitForSeconds(seconds);
        canPath = true;
        Debug.Log("Re-enabling pathing");
    }
}
