using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Stores the data in the inspector
*/

public class DeviceButtonSpritesObject : ScriptableObject
{
    [SerializeField] private InputDevice deviceType;
    public InputDevice DeviceType() { return deviceType; }

    public virtual Sprite GetSprite(string controlPath)
    {
        return null;
    }
}