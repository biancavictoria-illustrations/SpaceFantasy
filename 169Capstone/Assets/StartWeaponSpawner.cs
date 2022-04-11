using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWeaponSpawner : MonoBehaviour
{
    public GameObject dropItemPrefab;
    public EquipmentBaseData weaponData;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject itemObject = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        itemObject.GetComponent<GeneratedEquipment>().SetEquipmentBaseData( weaponData, ItemRarity.Common );
        itemObject.GetComponent<DropTrigger>().DropItemModelIn3DSpace();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
