using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform parent;
    public Transform spriteHolder;
    public new SpriteRenderer renderer;

    private Vector3 towardsCameraNoY;

    void Start()
    {
        if(!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();
        
        Vector3 towardsCamera = -Camera.main.transform.forward;
        towardsCameraNoY = new Vector3(towardsCamera.x, 0, towardsCamera.z).normalized;

        Vector3 tiltVector = Quaternion.FromToRotation(towardsCameraNoY, towardsCamera) * Vector3.up;
        float scale = 1 / Vector3.Project(tiltVector, Vector3.up).magnitude;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * scale, transform.localScale.z);
    }

    void Update()
    {
        if(spriteHolder)
        {
            //Face the sprite towards the camera
            spriteHolder.forward = towardsCameraNoY;
        }
    }
}
