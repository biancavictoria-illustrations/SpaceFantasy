using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FallingDebris : MonoBehaviour
{
    private const float startingHeight = 10;
    private const float gravity = 10;

    public float damage = 8;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private float currentVelocity;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.baseOffset = startingHeight;

        Vector3 spawnPosition = transform.position;
        Vector3 newPosition;
        NavMeshPath path = new NavMeshPath();
        do
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            float magnitude = Random.Range(1f, 20f);
            newPosition = spawnPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * magnitude;
        }
        while (!agent.CalculatePath(newPosition, path));
        agent.Warp(newPosition);

        rb = GetComponent<Rigidbody>();

        currentVelocity = 0;
    }

    void FixedUpdate()
    {
        currentVelocity += gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        agent.baseOffset -= currentVelocity * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<EntityHealth>().Damage(damage);
            Destroy(gameObject);
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject);
        }
    }
}
