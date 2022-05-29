using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmOfTheRam : Head
{
    private float damage;
    private float knockBack;
    private Player player;
    private Collider hitCollider;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        // damage = itemData.Damage() * player.stats.getSTRDamage();    // this can change depending on equips so we shouldn't save it, just get it each time
        // knockBack = 0.5f * player.stats.Strength();      // also should be calculated when used bc stats can change
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ResetItemAndTriggerCooldown()
    {
        StartCooldownRoutine();
    }

    private bool DetectCollision()
    {
        RaycastHit hit;

        if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, 3))
        {
            hitCollider = hit.collider;
            return true;
        }
        else
        {
            return false;
        }
    }
}
