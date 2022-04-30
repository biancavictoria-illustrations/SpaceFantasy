using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmOfTheRam : Head
{
    private float damage;
    private float knockBack;
    private Player player;
    private Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        damage = itemData.Damage() * player.stats.Strength();
        knockBack = 0.5f * player.stats.Strength();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool DetectCollision()
    {
        RaycastHit hit;

        if(Physics.Raycast(player.transform.position, player.transform.forward, out hit, 3))
        {
            collider = hit.collider;
            return true;
        }
        else
        {
            return false;
        }
    }
}
