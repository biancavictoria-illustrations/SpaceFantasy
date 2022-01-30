using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Enemy
{

    protected override IEnumerator EnemyLogic() //special
    {
        // Debug.Log("chasing");
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        // Debug.Log("Attacking");
        // path.attacking = true;
        // var animOptions = new string[] { "isShockwave", "isSlam" };
        // animator.SetTrigger(animOptions[Random.Range(0, animOptions.Length)]);
        // StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, logic.windUp, logic.duration, logic.windDown, logic.coolDown));
        // yield return new WaitUntil(() => baseAttack.Completed);
        // path.attacking = false;
        // coroutineRunning = false;
    }
}
