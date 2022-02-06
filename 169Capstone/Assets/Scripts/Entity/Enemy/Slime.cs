using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    protected override IEnumerator EnemyLogic()
    {
        animator.SetBool("IsMoving", true);
        yield return new WaitUntil(() => !path.Provoked() && !path.attacking);
        
        animator.SetBool("IsMoving", false);

        if(path.InAttackRange())
        {
            path.attacking = true;
            animator.SetTrigger("StartAttacking");
        }
        else
        {
            SetCooldown();
        }
    }

    public bool DealDamage()
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit, nextAttack.attackRange))
        {
            return hit.collider.tag == "Player" ? hit.collider.GetComponent<EntityHealth>().Damage(logic.baseDamage * nextAttack.damageMultiplier) : false;
        }
        else
        {
            return false;
        }
    }
}
