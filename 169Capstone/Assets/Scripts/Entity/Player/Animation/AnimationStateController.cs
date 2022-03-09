using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationStateController : MonoBehaviour
{
    public bool hitboxActive;
    public UnityEvent endAttack;

    public Animator animator;

    public void ActivateHitbox(int active)
    {
        hitboxActive = active > 0;
    }

    public void EndAttack()
    {
        endAttack.Invoke();
    }
}

