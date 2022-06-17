using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristRocket : Accessories
{
    private Movement movement;
    [SerializeField] GameObject rocketPrefab;

    [SerializeField][FMODUnity.EventRef] private string rocketFireSFX;
    [SerializeField][FMODUnity.EventRef] private string rocketImpactSFX;

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
        AudioManager.Instance.PlaySFX( rocketFireSFX, player.gameObject );
        
        GameObject rocket = Instantiate(rocketPrefab, player.transform.position + Vector3.up * 2, player.transform.rotation);
        Projectile projectileScript = rocket.GetComponent<Projectile>();
        projectileScript.Initialize("Enemy", player.stats.getINTDamage(), DamageSourceType.Player, InputManager.instance.cursorLookDirection, radius: data.equipmentBaseData.Radius(), speed: 30, projectileImpactSFX: rocketImpactSFX);
    }
}
