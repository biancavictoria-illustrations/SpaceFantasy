using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundPathing : MonoBehaviour
{
    private Transform player;
    private GameManager gameManager;
    private SpriteRenderer sprite;
    public NavMeshAgent agent;
    [HideInInspector] public float provokedRadius;
    [HideInInspector] public float attackRadius;
    [HideInInspector] public float speed = 1;
    [HideInInspector] public bool attacking = false;

    //private bool chase = false;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        if(sprite)
            sprite.transform.localScale = new Vector3(Random.value > 0.5f ? 1 : -1, sprite.transform.localScale.y, sprite.transform.localScale.z);
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
            gameManager = GameManager.instance;
        }
        
        float distance = Vector3.Distance(player.position, transform.position);

        if(distance < attackRadius || gameManager.inShopMode)
        {
            //Debug.Log("Stop Moving");
            agent.isStopped = true;
        }
        else if(distance < provokedRadius && !attacking)
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
            sprite.transform.localScale = new Vector3(-Mathf.Sign(Vector3.Dot(agent.velocity, Camera.main.transform.right)), sprite.transform.localScale.y, sprite.transform.localScale.z);
        }
        
    }

    public bool Provoked()
    {
        //Debug.Log()
        return !agent.isStopped;
    }

    public bool InAttackRange()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        return distance <= attackRadius;
    }
}
