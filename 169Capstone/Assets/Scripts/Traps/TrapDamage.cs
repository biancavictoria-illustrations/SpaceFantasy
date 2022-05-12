using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public float damage = 0.2f;
    public bool constDamage = true;

    public DamageSourceType damageSource;

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
