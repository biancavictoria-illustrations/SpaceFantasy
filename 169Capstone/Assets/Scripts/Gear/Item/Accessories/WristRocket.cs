using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristRocket : Accessories
{
    // Start is called before the first frame update
    void Start()
    {
        //GameObject player = GameObject.FindWithTag("Player");
        //damage = itemObject.damage * player.GetComponent<Player>().currentInt;
    }

    // Update is called once per frame
    void Update()
    {
        if(damage < 0)
        {
            GameObject player = GameObject.FindWithTag("Player");
            damage = itemObject.damage * player.GetComponent<Player>().currentInt;
        }
        if (fire && clearToFire)
        {
            fire = false;
            clearToFire = false;
            List<GameObject> hit = Fire();
            Damage(hit);
            StartCoroutine(CoolDown());
        }
        else if(fire)
        {
            fire = false;
        }
    }

    private List<GameObject> Fire()
    {
        List<GameObject> hit = new List<GameObject>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 explosionPoint = player.transform.position;

        explosionPoint.z += 10;

        for(int i = 0; i < enemies.Length; i++)
        {
            if(Vector3.Distance(explosionPoint, enemies[i].transform.position) <= itemObject.radius)
            {
                hit.Add(enemies[i]);
            }
        }

        return hit;
    }

    private void Damage(List<GameObject> enemies)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            base.Damage(enemies[i].GetComponent<EntityHealth>());
        }
    }
}
