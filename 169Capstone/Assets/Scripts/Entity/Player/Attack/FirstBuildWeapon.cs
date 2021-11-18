using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBuildWeapon : MonoBehaviour
{
    public float weaponRange = 2;
    public bool swing = false;
    public Transform player;
    public float damage = 1;
    //public int enemyLayer = 1 << 9;

    [SerializeField] private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(swing)
        {
            Collider[] enemiesHit = Physics.OverlapSphere(player.position, weaponRange, layerMask);

            if(enemiesHit.Length > 0)
            {
                Debug.Log(enemiesHit[0].name);
                damageEnemies(enemiesHit);
            }

            swing = false;
        }
    }

    private void damageEnemies(Collider[] enemies)
    {
        Debug.Log("Giving Damage");
        foreach(Collider enemy in enemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            enemyHealth.Hit(damage);
        }
    }


}
