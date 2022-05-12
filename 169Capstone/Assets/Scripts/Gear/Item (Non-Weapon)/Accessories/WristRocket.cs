using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristRocket : Accessories
{
    private Player player;
    private Movement movement;
    private AnimationStateController anim;
    //private bool fire = false;
    [SerializeField] GameObject rocketPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        movement = player.GetComponentInChildren<Movement>();
        anim = player.GetComponentInChildren<AnimationStateController>();
        damage = itemData.Damage() * player.stats.getINTDamage();
        anim.startAccessory.AddListener(LaunchRocket);
        anim.endAccessory.AddListener(ResetRocket);
    }

    // Update is called once per frame
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

    public void ResetRocket()
    {
        clearToFire = true;
    }

    private void Fire()
    {
        GameObject rocket = Instantiate(rocketPrefab, player.transform.position + Vector3.up * 2, player.transform.rotation);
        Projectile projectileScript = rocket.GetComponent<Projectile>();
        projectileScript.Initialize("Enemy", damage, DamageSourceType.Player, InputManager.instance.cursorLookDirection, itemData.Radius(), 30);
    }
}
