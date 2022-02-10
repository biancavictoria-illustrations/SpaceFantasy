using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robert : Enemy
{
    public Transform player;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    protected override void Start()
    {
        base.Start();

        if(player == null)
            player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

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
        GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.FromToRotation(transform.position, player.position));
        Projectile projectileScript = projectileObject.GetComponent<Projectile>();
        if(!projectileScript)
        {
            Destroy(projectileObject);
            Debug.LogError("Projectile prefab " + projectilePrefab + " did not contain a Projectile script.");
        }

        projectileScript.Initialize(LayerMask.NameToLayer("Player"), logic.baseDamage * nextAttack.damageMultiplier, player.position + Vector3.up*2 - projectileSpawnPoint.position);
    }
}
