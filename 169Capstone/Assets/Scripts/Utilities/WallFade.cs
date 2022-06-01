using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFade : MonoBehaviour
{
    private const float fadeDistance = 15f; // 1 grid unit is 20 units (at the time of writing)
    private const float fadeRate = 2f; // Speed is measured in percent change per second (1 = 100% change over the course of 1 second)
    private const float minAlpha = 0.5f; // The lowest percent that alpha can be
    private const float isometricPositionOffset = 5f; // The distance in the direction of the camera that the starting point should be offset

    private Vector3 towardsCameraNoY;
    private MeshRenderer renderer;
    [SerializeField] private Material transparentMat;
    [SerializeField] private Material opaqueMat;

    private float alpha;
    private bool shouldBeOpaque;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();

        // This makes it so that changing the transparency for this wall won't also change the transparency for every other wall in every scene for all time everywhere forever
        transparentMat = new Material(transparentMat);
        renderer.material = opaqueMat;

        alpha = 1;
        shouldBeOpaque = true;

        if(GameManager.instance.InSceneWithRandomGeneration())
        {
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(OnGenerationComplete);
        }
    }

    private void OnGenerationComplete()
    {
        Vector3 towardsCamera = -Camera.main.transform.forward;
        towardsCameraNoY = new Vector3(towardsCamera.x, 0, towardsCamera.z).normalized;
    }

    void Update()
    {
        if(!GameManager.instance.InSceneWithRandomGeneration())
            return;

        // If the player exists
        if(Player.instance != null)
        {
            // Get the vector from this wall's position toward the player
            Vector3 pointTowardPlayer = Player.instance.transform.position - transform.position;
            Vector3 pointTowardPlayerOffset = pointTowardPlayer + towardsCameraNoY * isometricPositionOffset;

            // If player is out of range OR is in front of the wall, the wall should be opaque
            shouldBeOpaque = (pointTowardPlayerOffset.magnitude > fadeDistance
                                || Vector3.Dot(transform.up, pointTowardPlayer) > 0);
        }

        if(shouldBeOpaque && alpha < 1)
        {
            alpha += fadeRate * Time.deltaTime;

            if(alpha >= 1)
            {
                alpha = 1;
                renderer.material = opaqueMat;
            }


            transparentMat.color = new Color(transparentMat.color.r, transparentMat.color.g, transparentMat.color.b, alpha);
        }

        if(!shouldBeOpaque && alpha > minAlpha)
        {
            if(renderer.material != transparentMat)
            {
                renderer.material = transparentMat;
            }

            alpha -= fadeRate * Time.deltaTime;
            
            if(alpha < minAlpha)
                alpha = minAlpha;
                
            transparentMat.color = new Color(transparentMat.color.r, transparentMat.color.g, transparentMat.color.b, alpha);
        }
    }
}
