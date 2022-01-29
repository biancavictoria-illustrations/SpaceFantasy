using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    private float damage;
    private int enemyLayer;

    public void Initialize(string enemyLayer, float damage, Vector3 direction, float? speed = null)
    {
        Initialize(LayerMask.NameToLayer(enemyLayer), damage, direction, speed);
    }

    public void Initialize(int enemyLayer, float damage, Vector3 direction, float? speed = null)
    {
        this.enemyLayer = enemyLayer;
        this.damage = damage;

        if(speed != null)
            this.speed = speed.Value;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            Destroy(gameObject);
            Debug.LogError("Projectile prefab " + gameObject + " did not contain a Rigidbody.");
        }

        rb.velocity = direction.normalized * this.speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == enemyLayer)
        {
            EntityHealth enemyHealth = other.GetComponent<EntityHealth>();
            if(enemyHealth)
                enemyHealth.Damage(damage);
            Destroy(gameObject);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
            Destroy(gameObject);
    }
}
