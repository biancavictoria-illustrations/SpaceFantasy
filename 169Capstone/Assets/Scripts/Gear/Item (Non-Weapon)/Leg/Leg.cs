using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : NonWeaponItem
{
    // Start is called before the first frame update
    void Start()
    {
        slot = 3;
    }

    private void Update()
    {
        if(InputManager.instance.useLegs)
        {
            fire = true;
        }
    }

    public IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(itemData.CoolDown());
        anim.animator.SetTrigger("CooldownLegs");
    }

    public IEnumerator Duration()
    {
        yield return new WaitForSeconds(itemData.Duration());
        anim.animator.SetTrigger("DurationLegs");
    }
}
