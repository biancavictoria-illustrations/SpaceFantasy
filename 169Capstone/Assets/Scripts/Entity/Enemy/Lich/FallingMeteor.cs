using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FallingMeteor : MonoBehaviour
{
    private const float startingHeight = 20;

    public float damage = 8;
    public float velocity = 20;

    private Rigidbody rb;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        Vector3 spawnPosition = transform.position;
        Vector3 newPosition;
        NavMeshPath path = new NavMeshPath();
        int count = 0;
        do
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            float magnitude = Random.Range(0f, 15f);
            newPosition = spawnPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * magnitude;
            ++count;
        }
        while (count < 100 && !agent.CalculatePath(newPosition, path));
        agent.Warp(newPosition);
        agent.enabled = false;

        transform.position += Vector3.up * startingHeight;
        rb.velocity = new Vector3(0, -velocity, 0);
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
