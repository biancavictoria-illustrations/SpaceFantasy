using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTrap : MonoBehaviour
{
    public MonoBehaviour trap;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy")
            trap.enabled = !trap.enabled;
    }
}
