using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private string currentControlScheme;
    
    void Start()
    {
        _controls = FindObjectOfType<PlayerInput>();
        OnControlSchemeChanged();
    }

    void Update()
    {
        if (_controls.currentControlScheme != currentControlScheme)
        {
            OnControlSchemeChanged();
            currentControlScheme = _controls.currentControlScheme;
        }
    }

    private void OnControlSchemeChanged()
    {
        if (_controls.currentControlScheme == "Gamepad"){
            if (currentControlDevice != InputDevice.Gamepad){
                currentControlDevice = InputDevice.Gamepad;
                UpdateInputDeviceValues();

                // Send Event
                // EventHandler.ExecuteEvent("DeviceChanged", currentControlDevice);
            }
        }
        else{
            if (currentControlDevice != InputDevice.KeyboardMouse){
                currentControlDevice = InputDevice.KeyboardMouse;
                UpdateInputDeviceValues();
                
                // Send Event
                // EventHandler.ExecuteEvent("DeviceChanged", currentControlDevice);
            }
        }
    }

    private void UpdateInputDeviceValues()
    {
        // Tell the InputManager we have a new input device
        InputManager.instance.UpdateLatestInputDevice();

        // Tell the controls UI it's changed
        ControlsMenu.inputDeviceChanged = true;

        // Also the alert text UI
        AlertTextUI.inputDeviceChanged = true;
    }
}
