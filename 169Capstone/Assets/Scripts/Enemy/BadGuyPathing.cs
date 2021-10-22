using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BadGuyPathing : MonoBehaviour
{
    public Transform player;
    public Transform self;
    public NavMeshAgent agent;
    public float chaseRadius = 7;
    public float stopRadius = 2;

    private bool chase = false;

    // Start is called before the first frame update
    void Start()
    {
        agent.isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        /*float distance = Vector3.Distance(player.position, self.position);
        if(distance < 2f)
        {
            agent.isStopped = true;
        }
        if(chase && distance > 2f)
        {
            // Pathing begins
            agent.SetDestination(player.position);
        }
        else if (Vector3.Distance(player.position, self.position) < radius && distance > 2f)
        {
            chase = true;
        }*/
        float distance = Vector3.Distance(player.position, self.position);

        if(distance < stopRadius)
        {
            agent.isStopped = true;
        }
        else if(distance < chaseRadius)
        {
            agent.isStopped = false;
        }

        if(!agent.isStopped)
        {
            agent.SetDestination(player.position);
        }
        
    }
}
