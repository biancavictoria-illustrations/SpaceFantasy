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

    private Coroutine destroyEffectCountdownRoutine;
    private GameObject circle;

    private FMOD.Studio.EventInstance snowstormLoopSFXEvent;
    [SerializeField][FMODUnity.EventRef] private string snowstormLoopSFX;
     
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void ActivateHelmet()
    {
        base.ActivateHelmet();
        // Save the instance of the snow circle
        circle = Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
        // find and save the vfx and scrip components
        snowVFX = circle.GetComponentInChildren<VisualEffect>();
        slowCircleScript = circle.GetComponentInChildren<SlowCircle>();
        hurtCircleScript = circle.GetComponentInChildren<HurtCircle>();

        // Initialize the scripts so they know what to do
        slowCircleScript.Initialize(Player.instance, "Enemy", "Pit", .20f, data.equipmentBaseData.Duration(), data.equipmentBaseData.Radius());
        hurtCircleScript.Initialize("Enemy", player.stats.getWISDamage(false).damageValue * data.equipmentBaseData.BaseDamage(), 
                                                data.equipmentBaseData.Duration(), DamageSourceType.Player, data.equipmentBaseData.Radius());
        
        // set the lifetime of the ground decal to the duration from the item
        snowVFX.SetFloat("GDLifetime", data.equipmentBaseData.Duration());

        // Setup & start the looping snowstorm SFX
        snowstormLoopSFXEvent = FMODUnity.RuntimeManager.CreateInstance(snowstormLoopSFX);
        snowstormLoopSFXEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(circle));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(snowstormLoopSFXEvent, circle.transform);
        snowstormLoopSFXEvent.start();

        destroyEffectCountdownRoutine = StartCoroutine(destroyEffectCountdown(data.equipmentBaseData.Duration(), fadeOutTime));
    }

    private IEnumerator destroyEffectCountdown(float lifetime, float fadeOutDuration = 0)   //GameObject circle, 
    {
        Debug.Log("Coroutine Started");
        // after the duration is over, stop vfx from spawning so they can fad out
        yield return new WaitForSeconds(lifetime - fadeOutDuration);
        circle.GetComponentInChildren<VisualEffect>().Stop();
        Debug.Log("VFX Stop Called");

        // after a delay destroy the circle, the damage and slow scripts will be destroyed but the vfx need time to dissapear
        yield return new WaitForSeconds(fadeOutDuration);
        snowstormLoopSFXEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Destroy(circle);
        Debug.Log("Destroy Called");
    }

    public override void ManageCoroutinesOnUnequip()
    {
        base.ManageCoroutinesOnUnequip();

        if(destroyEffectCountdownRoutine != null){
            StopCoroutine(destroyEffectCountdownRoutine);
            Destroy(circle);
        }
    }
}
