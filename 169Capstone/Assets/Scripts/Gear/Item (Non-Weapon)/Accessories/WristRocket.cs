using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristRocket : Accessories
{
    private Movement movement;
    [SerializeField] GameObject rocketPrefab;

    protected override void Start()
    {
        base.Start();

        movement = player.GetComponentInChildren<Movement>();
    }

    protected override void TriggerAbility()
    {
        base.TriggerAbility();
        
        if(clearToFire)
        {
            movement.isAttacking = true;
        }
    }

    protected override void ActivateAccessory()
    {
        base.ActivateAccessory();

        movement.isAttacking = false;
        Fire();
    }

    private void Fire()
    {
        GameObject rocket = Instantiate(rocketPrefab, player.transform.position + Vector3.up * 2, player.transform.rotation);
        Projectile projectileScript = rocket.GetComponent<Projectile>();
        projectileScript.Initialize("Enemy", player.stats.getINTDamage(), DamageSourceType.Player, InputManager.instance.cursorLookDirection, radius: itemData.Radius(), speed: 30);
    }
}
