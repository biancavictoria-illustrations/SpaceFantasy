using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robert : Enemy
{
    public Transform player;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    private Material defaultMat;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject explosionObject;

    protected override void Start()
    {
        base.Start();

        if(player == null)
            player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        EntityHealth healthScript = GetComponent<EntityHealth>();
        healthScript.OnHit.AddListener(FlashWhenHit);
        healthScript.OnDeath.AddListener(DeathAnimation);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        defaultMat = spriteRenderer.material;
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

        projectileScript.Initialize(LayerMask.NameToLayer("Player"), logic.baseDamage * nextAttack.damageMultiplier, DamageSourceType.Robert, player.position + Vector3.up*2 - projectileSpawnPoint.position);
    }

    private void FlashWhenHit(EntityHealth health, float damage)
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
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
    }
}
