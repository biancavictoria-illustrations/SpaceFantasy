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

    // TODO: "button" might become an image instead of a string
    private void SetAlertText(bool set, string button="", string newAlert="")
    {
        alert.SetActive(set);

        controlButton.text = button;
        alertText.text = newAlert;
    }

    // Called when you leave a trigger
    public void DisableAlert()
    {
        SetAlertText(false);
    }

    public void EnableInteractAlert()
    {
        SetAlertText(true, interactControl, "INTERACT");
    }

    public void EnableShopAlert()
    {
        SetAlertText(true, interactControl, "SHOP");
    }

    public void EnableProceedDoorAlert()
    {
        SetAlertText(true, interactControl, "PROCEED");
    }

    public void EnableItemPickupAlert()
    {
        SetAlertText(true, interactControl, "EXAMINE");
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
