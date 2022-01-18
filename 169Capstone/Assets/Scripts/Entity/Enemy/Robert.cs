using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robert : Enemy
{
    // Update is called once per frame
    void Update()
    {
        if(path.Provoked() && !coroutineRunning)
        {
            coroutineRunning = true;
            StartCoroutine(RobertLogic());
        }
    }

    private IEnumerator RobertLogic()
    {
        Timer timer = Instantiate(timerPrefab).GetComponent<Timer>();
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        path.attacking = true;

        for(int i = 0; i < 3; i++)
        {
            StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, logic.windUp, logic.duration, logic.windDown, logic.coolDown));
            StartCoroutine(CallDamage());
            timer.StartTimer(0.16f);
            yield return new WaitUntil(() => baseAttack.Completed && timer.timeRemaining <= 0);
        }

        path.attacking = false;
        coroutineRunning = false;
        Destroy(timer);
        //path.coro
        //StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, logic.windUp, logic.duration, logic.windDown, logic.coolDown));
    }

    private IEnumerator RobertLogic2()
    {
        animator.SetBool("IsMoving", true);
        //Debug.Log("chasing");
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        //Debug.Log("Attacking");
        animator.SetBool("IsMoving", false);
        path.attacking = true;
        animator.SetTrigger("StartAttacking");
    }
}
