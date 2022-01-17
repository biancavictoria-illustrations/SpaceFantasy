using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform spriteHolder;
    public new SpriteRenderer renderer;

    private Vector3 towardsCamera;

    void Start()
    {
        if(!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();
        
        towardsCamera = -Camera.main.transform.forward;

        Vector3 towardsCameraNoY = new Vector3(towardsCamera.x, 0, towardsCamera.z).normalized;
        
        //Adjust the transform to correct for the amount the sprite was tilted
        float tiltAmount = renderer.sprite.bounds.size.y * Mathf.Cos(Vector3.Angle(towardsCameraNoY, towardsCamera)) / 2;
        spriteHolder.position += towardsCameraNoY * tiltAmount;
    }

    void Update()
    {
        if(spriteHolder && renderer)
        {
            //Face the sprite towards the camera
            spriteHolder.forward = towardsCamera;
        }
    }
}
