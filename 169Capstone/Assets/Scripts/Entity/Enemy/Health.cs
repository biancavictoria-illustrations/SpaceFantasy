using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxhealth = 15;

    [SerializeField] private float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxhealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
    }
}
