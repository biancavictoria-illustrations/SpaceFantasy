using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FallingMeteor : MonoBehaviour
{
    private const float startingHeight = 10;

    public float damage = 8;
    public float velocity = 10;

    private Rigidbody rb;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.baseOffset = startingHeight;

        Vector3 spawnPosition = transform.position;
        Vector3 newPosition;
        NavMeshPath path = new NavMeshPath();
        int count = 0;
        do
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            float magnitude = Random.Range(0f, 10f);
            newPosition = spawnPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * magnitude;
            ++count;
        }
        while (count < 100 && !agent.CalculatePath(newPosition, path));
        agent.Warp(newPosition);

        rb = GetComponent<Rigidbody>();

        velocity = 0;
    }

    void FixedUpdate()
    {
        agent.baseOffset -= velocity * Time.fixedDeltaTime;
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
