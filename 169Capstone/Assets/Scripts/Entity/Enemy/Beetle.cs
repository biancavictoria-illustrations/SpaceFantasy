using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Enemy
{
    //private bool coroutineRunning = false;

    // Update is called once per frame
    void Update()
    {
        if(path.Provoked() && !coroutineRunning) // Update for damage later
        {
            //Debug.Log("in here");
            coroutineRunning = true;
            StartCoroutine(BossLogic());
            StartCoroutine(CallDamage());
        }
    }

    private IEnumerator BossLogic() //special
    {
        //Debug.Log("chasing");
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        //Debug.Log("Attacking");
        path.attacking = true;
        var animOptions = new string[] { "isShockwave", "isSlam" };
        animator.SetTrigger(animOptions[Random.Range(0, animOptions.Length)]);
        StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, logic.windUp, logic.duration, logic.windDown, logic.coolDown));
        yield return new WaitUntil(() => baseAttack.Completed);
        path.attacking = false;
        coroutineRunning = false;
    }
}
