using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    private Material defaultMat;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject explosionObject;

    protected override void Start()
    {
        base.Start();
        EntityHealth healthScript = GetComponent<EntityHealth>();
        healthScript.OnHit.AddListener(flashWhenHit);
        healthScript.OnDeath.AddListener(DeathAnimation);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        defaultMat = spriteRenderer.material;
    }

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
            float damageAmount = logic.baseDamage * nextAttack.damageMultiplier * (1 + damageIncreasePerTier * currentTier);
            return hit.collider.tag == "Player" ? hit.collider.GetComponent<EntityHealth>().Damage(damageAmount, DamageSourceType.Slime) : false;
        }
        else
        {
            return false;
        }
    }

    private void flashWhenHit(EntityHealth health, float damage)
    {
        Material flash = new Material(defaultMat);
        Material red = new Material(defaultMat);
        red.color = Color.red;
        spriteRenderer.material = flash;
        flash.Lerp(defaultMat, red, 1);
        Invoke("ResetMaterial", 0.05f);

    }

    private void ResetMaterial()
    {
        Material flash = spriteRenderer.material;
        Material red = new Material(defaultMat);
        red.color = Color.red;
        flash.Lerp(red, defaultMat, 1);
        spriteRenderer.material = defaultMat;
    }

    private void DeathAnimation(EntityHealth health)
    {
        GameObject explosion = Instantiate(explosionObject);
        Destroy(explosion, 3);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
}
