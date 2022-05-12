using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed
    {
        set { _speed = value; rb.velocity = rb.velocity.normalized * _speed; }

        get { return _speed; }
    }

    public int enemyLayer;

    [SerializeField] private float _speed;
    private float damage;
    private Rigidbody rb;
    private int _radius = 0;

    // Projectile's owner
    private DamageSourceType damageSource;

    public void Initialize(string enemyLayer, float damage, DamageSourceType damageSource, Vector3 direction, float? speed = null)
    {
        Initialize(LayerMask.NameToLayer(enemyLayer), damage, damageSource, direction, speed);
    }

    public void Initialize(string enemyLayer, float damage, DamageSourceType damageSource, Vector3 direction, int radius, float? speed = null)
    {
        _radius = radius;
        Initialize(LayerMask.NameToLayer(enemyLayer), damage, damageSource, direction, speed);
    }

    public void Initialize(int enemyLayer, float damage, DamageSourceType damageSource, Vector3 direction, float? speed = null)
    {
        rb = GetComponent<Rigidbody>();
        if(rb == null)
        {
            Destroy(gameObject);
            Debug.LogError("Projectile prefab " + gameObject + " did not contain a Rigidbody.");
            return;
        }

        this.enemyLayer = enemyLayer;
        this.damage = damage;
        this.damageSource = damageSource;

        if(speed != null)
            this._speed = speed.Value;

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if(renderer)
        {
            Vector2 lookDirection = Camera.main.WorldToScreenPoint(transform.position + direction) - Camera.main.WorldToScreenPoint(transform.position);
            renderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(lookDirection.y, lookDirection.x, 0).normalized);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        rb.velocity = direction.normalized * this._speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == enemyLayer)
        {
            EntityHealth enemyHealth = other.GetComponent<EntityHealth>();
            if(enemyHealth)
                enemyHealth.Damage(damage, damageSource);
            Explode();
            Destroy(gameObject);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Prop"))
        {
            other.GetComponent<PropJumpBreak>().BreakProp();
            Explode();
            if(_radius != 0)
            {
                Destroy(gameObject);
            }
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Explode();
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");

        for(int i = 0; i < enemies.Length; i++)
        {
            if(Vector3.Distance(transform.position, enemies[i].transform.position) <= _radius)
            {
                EntityHealth enemyHealth = enemies[i].GetComponent<EntityHealth>();
                if(enemyHealth)
                {
                    enemyHealth.Damage(damage, damageSource);
                }
            }
        }
        for(int i = 0; i < props.Length; i++)
        {
            if(Vector3.Distance(transform.position, props[i].transform.position) <= _radius)
            {
                props[i].GetComponent<PropJumpBreak>().BreakProp();
            }
        }
    }
}
