using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Item
{
    // Start is called before the first frame update
    void Start()
    {
        slot = 2;
    }

    private void Update()
    {
        if(InputManager.instance.useHead)
        {
            fire = true;
        }
    }
}
