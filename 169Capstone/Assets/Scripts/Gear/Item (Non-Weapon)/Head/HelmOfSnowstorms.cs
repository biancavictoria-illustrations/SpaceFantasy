using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmOfSnowstorms : Head
{
    // Start is called before the first frame update
    [SerializeField] private GameObject effectPrefab;
    private SlowCircle slowCircleScript;
    private HurtCircle hurtCircleScript;
     protected override void ActivateHelmet()
    {
        base.ActivateHelmet();
        GameObject circle = Instantiate(effectPrefab, player.transform.position, Quaternion.identity);
        slowCircleScript = circle.GetComponent<SlowCircle>();
        hurtCircleScript = circle.GetComponent<HurtCircle>();

        slowCircleScript.Initialize("Enemy", "Pit", .20f, itemData.Duration(), itemData.Radius());
        hurtCircleScript.Initialize("Enemy", player.stats.getWISDamage(false) * itemData.Damage(), 
                                                itemData.Duration(), DamageSourceType.Player, itemData.Radius());
    }
}
