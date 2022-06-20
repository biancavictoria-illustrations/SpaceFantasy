using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropJumpBreak : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
    }

    public virtual void BreakProp()
    {
        Destroy(gameObject);
    }
}
