using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : Item
{
    // Start is called before the first frame update
    void Start()
    {
        slot = 3;
    }

    private void Update()
    {
        if(InputManager.instance.useLegs)
        {
            fire = true;
        }
    }
}
