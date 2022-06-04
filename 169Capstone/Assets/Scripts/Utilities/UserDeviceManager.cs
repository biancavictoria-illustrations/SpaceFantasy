using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public enum InputDevice{
    KeyboardMouse,

    // Specific gamepad options (ideally implement different things.......)
    Gamepad,
    // XboxController,
    // PlaystationController,
    // SwitchController,
    // SwitchPro,
    
    // Anything else
    otherDevice
}

public class UserDeviceManager : MonoBehaviour
{
    PlayerInput _controls;
    public static InputDevice currentControlDevice;
    private static string currentControlScheme;

    public class InputDeviceChangedEvent : UnityEvent<InputDevice> {}
    public static InputDeviceChangedEvent OnInputDeviceChanged {get; private set;}
    
    void Awake()
    {
        OnInputDeviceChanged = new InputDeviceChangedEvent();
    }

    void Start()
    {
        _controls = FindObjectOfType<PlayerInput>();

        currentControlScheme = "Keyboard";
        OnControlSchemeChanged();
    }

    void Update()
    {
        if (_controls.currentControlScheme != currentControlScheme)
        {
            OnControlSchemeChanged();
        }
    }

    private void OnControlSchemeChanged()
    {
        if (_controls.currentControlScheme == "Gamepad"){
            if (currentControlDevice != InputDevice.Gamepad){
                currentControlDevice = InputDevice.Gamepad;
                currentControlScheme = _controls.currentControlScheme;
                OnInputDeviceChanged.Invoke(currentControlDevice);
            }
        }
        else{
            if (currentControlDevice != InputDevice.KeyboardMouse){
                currentControlDevice = InputDevice.KeyboardMouse;
                currentControlScheme = _controls.currentControlScheme;
                OnInputDeviceChanged.Invoke(currentControlDevice);
            }
        }
    }
}
