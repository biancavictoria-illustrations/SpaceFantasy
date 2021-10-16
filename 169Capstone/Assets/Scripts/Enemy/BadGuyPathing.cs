using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BadGuyPathing : MonoBehaviour
{
    public Transform player;
    public Transform self;
    public NavMeshAgent agent;
    public float radius = 2;

    private bool chase = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(chase)
        {
            // Pathing begins
            agent.SetDestination(player.position);
        }
        else if (Vector3.Distance(player.position, self.position) < radius)
        {
            chase = true;
        }
    }
}
