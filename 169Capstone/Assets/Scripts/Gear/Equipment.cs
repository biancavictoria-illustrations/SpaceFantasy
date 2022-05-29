using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class of all types of items, including weapons
public abstract class Equipment : MonoBehaviour
{
    // Generated when the item is spawned by GeneratedEquipment, passed in when the ACTUAL item is created here
    public GeneratedEquipment data {get; private set;}
    protected GameObject itemModel;

    // START & UPDATE both implemented in children -> if we put anything here, it would get overriden unless we call base.Start() / base.Update()
    // also AWAKE is implemented in NON-WEAPON children (Leg, Head, Accessories)
    
    public void SetEquipmentData(GeneratedEquipment _data)
    {
        data = _data;
    }

    public virtual void InitializeModel(GameObject itemModel)
    {
        this.itemModel = itemModel;
    }

    // Abstract methods implmeneted in children:

    public abstract void ManageCoroutinesOnUnequip();
}
