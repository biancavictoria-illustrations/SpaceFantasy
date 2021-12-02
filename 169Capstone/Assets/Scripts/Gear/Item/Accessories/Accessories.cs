using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessories : Item
{
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        slot = 1;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            fire = true;
        }
    }

    public void Damage(EntityHealth health)
    {
        health.Damage(damage);
    }
}
