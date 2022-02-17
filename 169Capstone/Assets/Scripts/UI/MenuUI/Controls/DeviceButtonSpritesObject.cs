using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Stores the data in the inspector
*/

public enum InputDeviceType{
    Xbox,
    PlayStation,
    Keyboard,
    Switch,
    SwitchPro,
    Other
}

public class DeviceButtonSpritesObject : ScriptableObject
{
    [SerializeField] private InputDeviceType deviceType;
    public InputDeviceType DeviceType() { return deviceType; }

    public virtual Sprite GetSprite(string controlPath)
    {
        return null;
    }
}