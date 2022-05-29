using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpinner : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, spinSpeed, 0, Space.Self);
    }
}
