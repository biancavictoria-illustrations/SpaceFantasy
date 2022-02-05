using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform parent;
    public Transform spriteHolder;
    public new SpriteRenderer renderer;

    private Vector3 towardsCamera;
    private Vector3 towardsCameraNoY;
    private Vector3 tiltOffsetVector;

    void Start()
    {
        if(!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();
        
        towardsCamera = -Camera.main.transform.forward;

        //Adjust the transform to correct for the amount the sprite was tilted
        towardsCameraNoY = new Vector3(towardsCamera.x, 0, towardsCamera.z).normalized;

        if(renderer)
        {
            float tiltAmount = renderer.sprite.bounds.size.y * Mathf.Cos(Vector3.Angle(towardsCameraNoY, towardsCamera)) / 2;
            tiltOffsetVector = towardsCameraNoY * tiltAmount;
        }
    }

    void Update()
    {
        if(spriteHolder)
        {
            //Face the sprite towards the camera
            spriteHolder.forward = towardsCamera;

            if(renderer)
                spriteHolder.position = parent.position + tiltOffsetVector;
        }
    }
}
