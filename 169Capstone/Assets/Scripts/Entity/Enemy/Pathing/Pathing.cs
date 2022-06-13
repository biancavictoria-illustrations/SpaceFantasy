using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Pathing : MonoBehaviour
{
    protected Transform player;
    protected GameManager gameManager;
    protected SpriteRenderer sprite;
    public NavMeshAgent agent;

    [HideInInspector] public float provokedRadius;
    [HideInInspector] public float attackRadius;
    [HideInInspector] public float speed = 1;
    [HideInInspector] public bool attacking = false;
    [HideInInspector] public bool canMove = true;

    protected abstract void HandleMovement();
    public abstract bool Provoked();
    public abstract bool InAttackRange();

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        agent.isStopped = true;

        //Start the enemy randomly flipped left or right
        if(sprite)
            sprite.flipX = Random.value > 0.5f;

        if(gameManager == null)
            gameManager = GameManager.instance;
    }

    void Update()
    {
        if(player == null)
            player = Player.instance.transform;

        HandleMovement();
    }
}
