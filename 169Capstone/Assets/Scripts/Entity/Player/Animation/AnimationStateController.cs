using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public bool attackActive;

    public Animator animator;

    public void ToggleAttackAnimation(bool set)
    {
        animator.SetBool("IsAttacking", set);
        if(set)
        {
            Debug.Log(animator.GetLayerIndex("Slashing"));
            animator.SetBool("IsAttacking", true);
        }
    }

    public void ActivateAttack()
    {
        attackActive = true;
    }
}

