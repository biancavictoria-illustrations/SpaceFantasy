using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform parent;
    public Transform spriteHolder;
    public new SpriteRenderer renderer;

    private Vector3 towardsCameraNoY;

    public static bool generationComplete;

    void Awake()
    {
        // If main level (do this in awake to add a listener)
        if(GameManager.instance.InSceneWithRandomGeneration()){
            FloorGenerator generator = FindObjectOfType<FloorGenerator>();
            if(generator){  // If we already completed generation, just run start; if not, add a listener
                if(generator.generationComplete){
                    StartOnGenerationComplete();
                }
                else{
                    generator.OnGenerationComplete.AddListener(StartOnGenerationComplete);
                }
            }
        }
    }

    void Start()
    {
        // If main hub or something
        if(!GameManager.instance.InSceneWithRandomGeneration()){
            StartOnGenerationComplete();
        }
    }

    private void StartOnGenerationComplete()
    {
        if(!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();
        
        Vector3 towardsCamera = -Camera.main.transform.forward;
        towardsCameraNoY = new Vector3(towardsCamera.x, 0, towardsCamera.z).normalized;

        Vector3 tiltVector = Quaternion.FromToRotation(towardsCameraNoY, towardsCamera) * Vector3.up;
        float scale = 1 / Vector3.Project(tiltVector, Vector3.up).magnitude;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * scale, transform.localScale.z);

        generationComplete = true;
    }

    void Update()
    {
        if(spriteHolder && generationComplete && towardsCameraNoY != Vector3.zero)
        {
            //Face the sprite towards the camera
            spriteHolder.forward = towardsCameraNoY;
        }
    }
}
