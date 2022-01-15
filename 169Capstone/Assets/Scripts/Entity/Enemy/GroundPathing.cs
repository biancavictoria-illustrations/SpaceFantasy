using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundPathing : MonoBehaviour
{
    private Transform player;
    private GameManager gameManager;
    public Transform self;
    public NavMeshAgent agent;
    [HideInInspector] public float provokedRadius;
    [HideInInspector] public float attackRadius;
    [HideInInspector] public float speed = 1;
    [HideInInspector]public bool attacking = false;

    //private bool chase = false;

    // Start is called before the first frame update
    void Start()
    {
        agent.isStopped = true;
        //player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(agent.isStopped);
        if(player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
        if(gameManager == null)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        
        float distance = Vector3.Distance(player.position, self.position);

        if(distance < attackRadius || gameManager.inShopMode)
        {
            //Debug.Log("Stop Moving");
            agent.isStopped = true;
        }
        else if(distance < provokedRadius && !attacking && !gameManager.inShopMode)
        {
            //Debug.Log("Start Moving");
            agent.isStopped = false;
        }

        if(!agent.isStopped)
        {
            //Debug.Log("Chase");
            //Debug.Log(player.position);
            agent.speed = speed;
            //Debug.Log(agent.speed);
            //agent.CalculatePath(player)
            agent.SetDestination(player.position);
            //agent.CalculatePath(player.position, agent.path);
            //agent.destination = player.position;
        }
        
    }

    public bool Provoked()
    {
        //Debug.Log()
        return !agent.isStopped;
    }

    public bool InAttackRange()
    {
        float distance = Vector3.Distance(player.position, self.position);
        return distance <= attackRadius;
    }
}
