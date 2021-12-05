using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessories : Item
{
    [HideInInspector] public float damage = -1;

    // Start is called before the first frame update
    void Start()
    {
        slot = 1;
    }

    private void Update()
    {
        /*if(Input.GetButtonDown("Fire2"))
        {
            fire = true;
        }*/
        if(InputManager.instance.useAccessory)
        {
            Debug.Log("fire");
            fire = true;
        }
    }

    public void Damage(EntityHealth health)
    {
        health.Damage(damage);
    }
}
