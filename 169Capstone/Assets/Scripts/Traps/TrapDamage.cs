using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public float damage = 0.2f;
    public bool constDamage = true;

    public DamageSourceType damageSource;

    private FMOD.Studio.EventInstance trapLoopingSFXEvent;
    [SerializeField][FMODUnity.EventRef] private string trapLoopingSFX;

    void Start()
    {
        if(GameManager.instance.InSceneWithRandomGeneration()){
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(StartOnGenerationComplete);
        }
        else{
            StartOnGenerationComplete();
        }
    }

    void OnDisable()
    {
        StopLoopingSFX();
    }

    private void StartOnGenerationComplete()
    {
        if(trapLoopingSFX != ""){
            // Setup and start SFX
            trapLoopingSFXEvent = FMODUnity.RuntimeManager.CreateInstance(trapLoopingSFX);
            trapLoopingSFXEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(trapLoopingSFXEvent, transform);
            trapLoopingSFXEvent.start();
        }
    }

    public void StopLoopingSFX()
    {
        if(trapLoopingSFX != ""){
            trapLoopingSFXEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<EntityHealth>() != null)
        {
            other.GetComponent<EntityHealth>().Damage(damage, damageSource);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(constDamage)
        {
            if (other.GetComponent<EntityHealth>() != null)
            {
                other.GetComponent<EntityHealth>().Damage(damage, damageSource);
            }
        }
    }
}
