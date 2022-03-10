using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Parent class of all types of items, including weapons
public abstract class Equipment : MonoBehaviour
{
    // Generated when the item is spawned by GeneratedEquipment, passed in when the ACTUAL item is created here
    public SpawnedEquipmentData data {get; private set;}
    protected GameObject itemModel;
    
    public void SetEquipmentData(SpawnedEquipmentData _data)
    {
        data = _data;
    }

    public virtual void InitializeModel(GameObject itemModel)
    {
        this.itemModel = itemModel;
    }

    // Contains any abstract methods to be implmeneted in the children
}
