using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HelmOfSnowstorms : Head
{
    // Start is called before the first frame update
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float fadeOutTime = 1f;
    private SlowCircle slowCircleScript;
    private HurtCircle hurtCircleScript;
    private VisualEffect snowVFX;
     
     protected override void Awake()
     {
        base.Awake();
     }
     protected override void ActivateHelmet()
    {
        base.ActivateHelmet();
        // Save the instance of the snow circle
        GameObject circle = Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
        // find and save the vfx and scrip components
        snowVFX = circle.GetComponentInChildren<VisualEffect>();
        slowCircleScript = circle.GetComponentInChildren<SlowCircle>();
        hurtCircleScript = circle.GetComponentInChildren<HurtCircle>();

        // Initialize the scripts so they know what to do
        slowCircleScript.Initialize("Enemy", "Pit", .20f, itemData.Duration(), itemData.Radius());
        hurtCircleScript.Initialize("Enemy", player.stats.getWISDamage(false) * itemData.Damage(), 
                                                itemData.Duration(), DamageSourceType.Player, itemData.Radius());
        
        // set the lifetime of the ground decal to the duration from the item
        snowVFX.SetFloat("GDLifetime", itemData.Duration());

        StartCoroutine(destroyEffectCountdown(itemData.Duration(), circle, fadeOutTime));
    }

    private IEnumerator destroyEffectCountdown(float lifetime, GameObject circle, float fadeOutDuration = 0)
    {
        Debug.Log("Coroutine Started");
        // after the duration is over, stop vfx from spawning so they can fad out
        yield return new WaitForSeconds(lifetime - fadeOutDuration);
        circle.GetComponentInChildren<VisualEffect>().Stop();
        Debug.Log("VFX Stop Called");

        // after a delay destroy the circle, the damage and slow scripts will be destroyed but the vfx need time to dissapear
        yield return new WaitForSeconds(fadeOutDuration);
        Destroy(circle);
        Debug.Log("Destroy Called");
    }
}
