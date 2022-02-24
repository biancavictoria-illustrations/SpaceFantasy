using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropJumpBreak : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
    }

    public void BreakProp()
    {
        Destroy(gameObject);
    }
}
