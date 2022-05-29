using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristRocket : Accessories
{
    private Player player;
    private Movement movement;
    //private AnimationStateController anim;
    //private bool fire = false;
    [SerializeField] GameObject rocketPrefab;

    void Start()
    {
        player = Player.instance;
        movement = player.GetComponentInChildren<Movement>();
        anim = player.GetComponentInChildren<AnimationStateController>();
        // damage = itemData.Damage() * player.stats.getINTDamage();    // this can change depending on equips so we shouldn't save it in start, just get it each time
        anim.startAccessory.AddListener(LaunchRocket);
        anim.endAccessory.AddListener(ResetItemAndTriggerCooldown);
    }

    void Update()
    {
        if(InputManager.instance.useAccessory && clearToFire)
        {
            clearToFire = false;
            movement.isAttacking = true;
            anim.animator.SetBool("IsLaunchRocket", true);
            //fire = true;
            InputManager.instance.useAccessory = false; // temp
        }
    }

    public void LaunchRocket()
    {
        Fire();
        anim.animator.SetBool("IsLaunchRocket", false);
        movement.isAttacking = false;
    }

    public override void ResetItemAndTriggerCooldown()
    {
        clearToFire = true;
        StartCooldownRoutine();
    }

    private void Fire()
    {
        GameObject rocket = Instantiate(rocketPrefab, player.transform.position + Vector3.up * 2, player.transform.rotation);
        Projectile projectileScript = rocket.GetComponent<Projectile>();
        projectileScript.Initialize("Enemy", player.stats.getINTDamage(), DamageSourceType.Player, InputManager.instance.cursorLookDirection, itemData.Radius(), 30);
    }
}
