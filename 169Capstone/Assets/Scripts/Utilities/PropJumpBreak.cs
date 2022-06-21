using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropJumpBreak : MonoBehaviour
{
    [SerializeField] private GameObject explosionObject;
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
           DestroyProp();
        }
    }

    public virtual void BreakProp()
    {
        DestroyProp();
    }

     private void DestroyProp()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.BreakProp, gameObject);
        GameObject explosion = Instantiate(explosionObject);
        Destroy(explosion, 3);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
         Destroy(gameObject);
    }
}
