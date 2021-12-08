using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Fell onto trigger");
        if(other.tag == "Player")
        {
            EntityHealth playerHealth = other.GetComponent<EntityHealth>();
            playerHealth.Damage(playerHealth.currentHitpoints);
        }
    }
}
