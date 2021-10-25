using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //animator.SetBool("IsAttacking", true);
            //Debug.Log(animator.GetLayerIndex("Slashing"));
            animator.SetLayerWeight(1, 1);

        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
        
    }

    public void TurnOffAttack()
    {
        Debug.Log("Done attacking");
        //animator.SetBool("IsAttacking", false);
        animator.SetLayerWeight(1, 0);
    }

}

