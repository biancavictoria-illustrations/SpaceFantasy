using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWeaponSpawner : MonoBehaviour
{
    public GameObject dropItemPrefab;
    public EquipmentBaseData weaponData;

    [HideInInspector] public GameObject itemObject;

    [Tooltip("ONLY False for run 1 variant spawn room sword; True otherwise")]
    public bool dropIn3DSpace = true;
    
    void Start()
    {
        itemObject = Instantiate(dropItemPrefab, transform.position, Quaternion.identity, transform);

        if(weaponData){
            itemObject.GetComponent<GeneratedEquipment>().SetEquipmentBaseData( weaponData, ItemRarity.Common );
        }
        else{
            Debug.LogWarning("No weapon data found for StartWeaponSpawner");
        }

        // Unless this auto-equips to the player, this should be true and we drop it in the level space
        if(dropIn3DSpace){
            itemObject.GetComponent<DropTrigger>().DropItemModelIn3DSpace();
        }        
    }
}
