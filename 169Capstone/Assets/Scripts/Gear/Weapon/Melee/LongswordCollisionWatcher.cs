using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LongswordCollisionWatcher : MonoBehaviour
{
    [System.Serializable]
    public class OnHitEvent : UnityEvent<Collider>{}

    public UnityEvent<Collider> hitEvent;

    void Awake()
    {
        hitEvent = new OnHitEvent();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit " + other);

        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Prop"))
            hitEvent.Invoke(other);
    }
}
