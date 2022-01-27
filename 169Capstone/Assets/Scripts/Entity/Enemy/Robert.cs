using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robert : Enemy
{
    public GameObject projectilePrefab;

    protected override IEnumerator EnemyLogic() //special
    {
        animator.SetBool("IsMoving", true);
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        
        animator.SetBool("IsMoving", false);
        path.attacking = true;
        animator.SetTrigger("StartAttacking");
    }

    public void ShootProjectile()
    {

    }
}
