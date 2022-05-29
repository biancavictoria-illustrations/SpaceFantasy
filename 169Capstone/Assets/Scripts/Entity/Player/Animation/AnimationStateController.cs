using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationStateController : MonoBehaviour
{
    public bool attackActive;
    public UnityEvent endAttack;
    public UnityEvent startAccessory;
    public UnityEvent endCooldownAccessory;
    public UnityEvent endAccessory;
    public UnityEvent startLegs;
    public UnityEvent endLegs;
    public UnityEvent endCooldownLegs;

    public Animator animator;

    [SerializeField] private GameObject swordSlashVFX;

    public void ActivateHitbox(int active)
    {
        attackActive = active > 0;
        if (active == 1)
        {
            GameObject vfx = Instantiate(swordSlashVFX, Player.instance.transform.position,transform.rotation);
            Destroy(vfx, 1);
        }
    }

    public void EndAttack()
    {
        endAttack.Invoke();
    }

    // Use the ability, triggers duration
    public void StartAccessory()
    {
        startAccessory.Invoke();
    }

    // Reset, triggers cooldown
    public void EndAccessory()
    {
        endAccessory.Invoke();
    }

    // Ends the cooldown itself, can use ability now
    public void EndCoolDownAccessory()
    {
        endCooldownAccessory.Invoke();
    }


    // Use the ability, triggers duration
    public void StartLegs()
    {
        startLegs.Invoke();
    }

    // Reset, triggers cooldown
    public void EndLegs()
    {
        endLegs.Invoke();
    }

    // Ends the cooldown itself, can use ability now
    public void EndCoolDownLegs()
    {
        endCooldownLegs.Invoke();
    }
}

