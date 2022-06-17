using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public float damage = 0.2f;
    public bool constDamage = true;

    public DamageSourceType damageSource;

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

    public void StartOnGenerationComplete()
    {
        if(trapLoopingSFX != "")
            AudioManager.Instance.PlaySFX(trapLoopingSFX, gameObject);
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
