using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class AlertTextUI : MonoBehaviour
{
    public static AlertTextUI instance;

    [SerializeField] private GameObject alert;
    [SerializeField] private TMP_Text controlButton;
    [SerializeField] private TMP_Text alertText;

    public bool alertTextIsActive = false;

    public static bool inputDeviceChanged = false;

    private string interactControl; // TODO: Will likely become an icon later...?
    [SerializeField] private InputActionReference interactAction;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    void Update()
    {
        if(inputDeviceChanged){
            UpdateAlertText();
            inputDeviceChanged = false;
        }
    }

    // TODO: "button" might become an image instead of a string
    private void SetAlertText(bool set, string button="", string newAlert="")
    {
        ToggleAlertText(set);

        controlButton.text = button;
        alertText.text = newAlert;
    }

    // For temporarily toggling in pause menu and other screens like that (if necessary)
    public void ToggleAlertText(bool set)
    {
        alert.SetActive(set);
    }

    // Called when you leave a trigger
    public void DisableAlert()
    {
        SetAlertText(false);
        alertTextIsActive = false;
    }

    public void EnableInteractAlert()
    {
        SetAlertText(true, interactControl, "INTERACT");
        alertTextIsActive = true;
    }

    public void EnableShopAlert()
    {
        SetAlertText(true, interactControl, "SHOP");
        alertTextIsActive = true;
    }

    public void EnableProceedDoorAlert()
    {
        SetAlertText(true, interactControl, "PROCEED");
        alertTextIsActive = true;
    }

    public void EnableItemPickupAlert()
    {
        SetAlertText(true, interactControl, "EXAMINE");
        alertTextIsActive = true;
    }

    public void UpdateAlertText()
    {
        interactControl = GetActionIcon(interactAction);
        controlButton.text = interactControl;   // Will need to change this if there's more than just this one...
    }

    private string GetActionIcon(InputActionReference action)
    {
        int bindingIndex = action.action.GetBindingIndexForControl(action.action.controls[0]);
        return InputControlPath.ToHumanReadableString(action.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}
