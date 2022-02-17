using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controls/GamepadButtonSprites")]
public class GamepadButtonSprites : DeviceButtonSpritesObject
{
    [SerializeField] private Sprite buttonSouth;
    [SerializeField] private Sprite buttonNorth;
    [SerializeField] private Sprite buttonEast;
    [SerializeField] private Sprite buttonWest;
    [SerializeField] private Sprite startButton;
    [SerializeField] private Sprite selectButton;
    [SerializeField] private Sprite leftTrigger;
    [SerializeField] private Sprite rightTrigger;
    [SerializeField] private Sprite leftShoulder;
    [SerializeField] private Sprite rightShoulder;
    [SerializeField] private Sprite dpad;
    [SerializeField] private Sprite dpadUp;
    [SerializeField] private Sprite dpadDown;
    [SerializeField] private Sprite dpadLeft;
    [SerializeField] private Sprite dpadRight;
    [SerializeField] private Sprite leftStick;
    [SerializeField] private Sprite rightStick;
    [SerializeField] private Sprite leftStickPress;
    [SerializeField] private Sprite rightStickPress;

    public override Sprite GetSprite(string controlPath)
    {
        // From the input system, we get the path of the control on device. So we can just
        // map from that to the sprites we have for gamepads.
        switch (controlPath)
        {
            case "Button South": return buttonSouth;
            case "Button North": return buttonNorth;
            case "Button East": return buttonEast;
            case "Button West": return buttonWest;
            case "Start": return startButton;
            case "Select": return selectButton;

            case "Left Trigger": return leftTrigger;
            case "Right Trigger": return rightTrigger;
            case "Left Shoulder": return leftShoulder;
            case "Right Shoulder": return rightShoulder;

            case "D-Pad": return dpad;
            case "D-Pad/Up": return dpadUp;
            case "D-Pad/Down": return dpadDown;
            case "D-Pad/Left": return dpadLeft;
            case "D-Pad/Right": return dpadRight;

            case "Left Stick": return leftStick;
            case "Left Stick/Down": return leftStick;
            case "Left Stick/Right": return leftStick;
            case "Left Stick/Left": return leftStick;
            case "Left Stick/Up": return leftStick;
            case "Left Stick Press": return leftStickPress;

            case "Right Stick": return rightStick;
            case "Right Stick/Down": return rightStick;
            case "Right Stick/Right": return rightStick;
            case "Right Stick/Left": return rightStick;
            case "Right Stick/Up": return rightStick;
            case "Right Stick Press": return rightStickPress;
        }
        Debug.LogWarning("No icon found for control path: " + controlPath);
        return null;
    }
}
