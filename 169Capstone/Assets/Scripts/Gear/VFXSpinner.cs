using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpinner : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1;
    [SerializeField] private float spinAxis = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(spinAxis == 1)
        {
            transform.Rotate(spinSpeed, 0,  0, Space.Self);
        }
        else if(spinAxis == 2)
        {
            transform.Rotate(0, spinSpeed, 0, Space.Self);
        }
        else
        {
            transform.Rotate(0,  0, spinSpeed, Space.Self);
        }
    }
}
