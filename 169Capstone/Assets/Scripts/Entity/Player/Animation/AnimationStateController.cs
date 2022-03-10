using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationStateController : MonoBehaviour
{
    public bool attackActive;
    public UnityEvent endAttack;

    public Animator animator;

    public void ActivateHitbox(int active)
    {
        attackActive = active > 0;
    }

    public void EndAttack()
    {
        endAttack.Invoke();
    }
}

