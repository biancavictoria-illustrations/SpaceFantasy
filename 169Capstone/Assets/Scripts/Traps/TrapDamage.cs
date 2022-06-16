using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public float damage = 0.2f;
    public bool constDamage = true;

    public DamageSourceType damageSource;

    [SerializeField][FMODUnity.EventRef] private string trapSFX;
    // private FMOD.Studio.EventInstance trapSFXEvent;

    void Start()
    {
        if(GameManager.instance.InSceneWithRandomGeneration()){
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(StartOnGenerationComplete);
        }
        else{
            StartOnGenerationComplete();
        }
    }

    public void StartOnGenerationComplete()
    {
        // trapSFXEvent = FMODUnity.RuntimeManager.CreateInstance(trapSFX);
        // AudioManager.Instance.SetupSFXOnStart(trapSFX, trapSFXEvent, gameObject);
        // AudioManager.Instance.PlaySFX(trapSFX, trapSFXEvent, gameObject);
        if(trapSFX != "")
            AudioManager.Instance.PlaySFX(trapSFX, gameObject);
    }

    // Start is called before the first frame update
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
