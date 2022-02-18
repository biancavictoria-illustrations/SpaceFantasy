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
        Transform player = Player.instance.transform;
        if(Physics.Raycast(transform.position + Vector3.up, player.position - transform.position, out hit, nextAttack.attackRange))
        {
            return hit.collider.tag == "Player" ? hit.collider.GetComponent<EntityHealth>().Damage(logic.baseDamage * nextAttack.damageMultiplier) : false;
        }
        else
        {
            return false;
        }
    }
}
