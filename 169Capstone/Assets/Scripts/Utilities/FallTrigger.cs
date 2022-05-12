using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //collision.
        Debug.Log(collision.collider.tag);
        if(collision.collider.tag == "Player")
        {
            Debug.Log("Player fell onto trigger");
            EntityHealth playerHealth = collision.collider.GetComponent<EntityHealth>();
            playerHealth.Damage(playerHealth.currentHitpoints, DamageSourceType.DeathPit);
        }
    }
}
