using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputDevice{
    KeyboardMouse,

    // Specific gamepad options (ideally implement different things.......)
    Gamepad,
    // XboxController,
    // PlaystationController,
    // SwitchController,
    
    // Anything else
    otherDevice
}

public class UserDeviceManager : MonoBehaviour
{
    UnityEngine.InputSystem.PlayerInput _controls;
    public static InputDevice currentControlDevice;
    
    void Start()
    {
        _controls = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _controls.onControlsChanged += OnControlsChanged;
    }
    
    private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput obj)
    {
        if (obj.currentControlScheme == "Gamepad"){
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
