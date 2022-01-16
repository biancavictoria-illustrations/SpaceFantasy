using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    //private bool coroutineRunning = false;

    // Update is called once per frame
    void Update()
    {
        if(path.Provoked() && !coroutineRunning && canAttack) // Update for damage later
        {
            //Debug.Log("in here");
            coroutineRunning = true;
            StartCoroutine(SlimeLogic2());
            //StartCoroutine(CallDamage());
        }

        if(path.Provoked())
        {
            //Debug.Log(transform.position);
        }
    }

    private IEnumerator SlimeLogic() //special
    {
        animator.SetBool("IsMoving", true);
        //Debug.Log("chasing");
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        //Debug.Log("Attacking");
        path.attacking = true;
        animator.SetTrigger("StartAttacking");
        StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, logic.windUp, logic.duration, logic.windDown, logic.coolDown));
        yield return new WaitUntil(() => baseAttack.Completed);
        path.attacking = false;
        coroutineRunning = false;
        animator.SetBool("IsMoving", false);
    }

    private IEnumerator SlimeLogic2() //special
    {
        animator.SetBool("IsMoving", true);
        //Debug.Log("chasing");
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        //Debug.Log("Attacking");
        animator.SetBool("IsMoving", false);
        path.attacking = true;
        animator.SetTrigger("StartAttacking");
        //StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, logic.windUp, logic.duration, logic.windDown, logic.coolDown));
        //yield return new WaitUntil(() => baseAttack.Completed);
        //animator.SetBool("InCoolDown", true);
        //path.attacking = false;
        //coroutineRunning = false;
        //animator.SetBool("IsMoving", false);
    }
}
