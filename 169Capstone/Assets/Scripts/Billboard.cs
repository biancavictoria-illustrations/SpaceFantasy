using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform spriteHolder;

    void Update()
    {
        if(spriteHolder)
        {
            spriteHolder.forward = -Camera.main.transform.forward;
        }
    }
}
