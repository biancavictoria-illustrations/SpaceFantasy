using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationStateController : MonoBehaviour
{
    public bool attackActive;
    public UnityEvent endAttack;
    public UnityEvent startAccessory;
    public UnityEvent startCooldownAccessory;
    public UnityEvent endAccessory;
    public UnityEvent startLegs;
    public UnityEvent endLegs;
    public UnityEvent startCooldownLegs;

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

    public void StartAccessory()
    {
        startAccessory.Invoke();
    }

    public void EndAccessory()
    {
        endAccessory.Invoke();
    }

    public void StartCoolDownAccessory()
    {
        startCooldownAccessory.Invoke();
    }

    public void StartLegs()
    {
        startLegs.Invoke();
    }

    public void EndLegs()
    {
        endLegs.Invoke();
    }

    public void StartCoolDownLegs()
    {
        startCooldownLegs.Invoke();
    }
}

