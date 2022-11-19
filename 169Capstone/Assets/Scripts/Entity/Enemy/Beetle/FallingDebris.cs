using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class FallingDebris : MonoBehaviour
{
    private const float startingHeight = 20;
    private const float gravity = 15;

    public float damage = 8;

    [SerializeField] private GameObject debrisIndicatorVFX;
    [SerializeField] private GameObject explosionObject;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private float currentVelocity;

    [SerializeField][FMODUnity.EventRef] private string debrisImpactSFX;

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
            float magnitude = Random.Range(5f, 30f);
            newPosition = spawnPosition + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * magnitude;
            ++count;
        }
        while (count < 100 && !agent.CalculatePath(newPosition, path));
        agent.Warp(newPosition);
        agent.enabled = false;

        GameObject indicator = Instantiate(debrisIndicatorVFX, transform.position, Quaternion.identity);
        VisualEffect vfx = indicator.GetComponent<VisualEffect>();
        float fallTime = Mathf.Sqrt(2 * startingHeight / gravity);
        vfx.SetFloat("GDLifetime", fallTime);
        Destroy(indicator, fallTime);

        transform.position += Vector3.up * startingHeight;
        currentVelocity = 0;
    }

    void FixedUpdate()
    {
        currentVelocity += gravity * Time.fixedDeltaTime;
        rb.velocity = new Vector3(0, -currentVelocity, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            AudioManager.Instance.PlaySFX(debrisImpactSFX, gameObject);
            other.GetComponent<EntityHealth>().Damage(new DamageData(damage,false), DamageSourceType.BeetleBoss);
            Destroy(gameObject);
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            AudioManager.Instance.PlaySFX(debrisImpactSFX, gameObject);
            Destroy(gameObject);
        }
    }

     void OnDestroy()
    {
        GameObject explosion = Instantiate(explosionObject);
        Destroy(explosion, 3);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
}
