using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropJumpBreak : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            AudioManager.Instance.PlaySFX(AudioManager.SFX.BreakProp, gameObject);
            Destroy(gameObject);
        }
    }

    public virtual void BreakProp()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.BreakProp, gameObject);
        Destroy(gameObject);
    }
}
