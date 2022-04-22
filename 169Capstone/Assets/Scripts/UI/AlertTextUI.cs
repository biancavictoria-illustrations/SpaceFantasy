using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AlertTextUI : MonoBehaviour
{
    public static AlertTextUI instance;

    [SerializeField] private GameObject alert;
    [SerializeField] private Image controlButtonIcon;
    [SerializeField] private TMP_Text controlButtonText;
    [SerializeField] private TMP_Text alertText;

    [HideInInspector] public bool alertTextIsActive = false;

    public static bool inputDeviceChanged = false;

    private Sprite interactControlIcon;
    private string interactControlString;
    
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

    void Start()
    {
        alertTextIsActive = false;
        UpdateAlertText();
    }

    void Update()
    {
        if(inputDeviceChanged){
            UpdateAlertText();
            inputDeviceChanged = false;
        }
    }

    private void SetAlertText(bool set, Sprite _icon=null, string _text="", string newAlertDescription="")
    {
        ToggleAlertText(set);

        if(_icon){
            controlButtonIcon.gameObject.SetActive(true);
            controlButtonText.gameObject.SetActive(false);

            controlButtonIcon.sprite = _icon;
            controlButtonIcon.preserveAspect = true;
        }
        else{
            controlButtonIcon.gameObject.SetActive(false);
            controlButtonText.gameObject.SetActive(true);
            
            controlButtonText.text = _text;
        }

        alertText.text = newAlertDescription;
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
        SetAlertText(true, interactControlIcon, interactControlString, "INTERACT");
        alertTextIsActive = true;
    }

    public void EnableShopAlert()
    {
        SetAlertText(true, interactControlIcon, interactControlString, "SHOP");
        alertTextIsActive = true;
    }

    public void EnableProceedDoorAlert()
    {
        SetAlertText(true, interactControlIcon, interactControlString, "PROCEED");
        alertTextIsActive = true;
    }

    public void EnableItemPickupAlert()
    {
        SetAlertText(true, interactControlIcon, interactControlString, "EXAMINE");
        alertTextIsActive = true;
    }

    // If showing alerts for more than just interact, UPDATE this to include those as well
    public void UpdateAlertText()
    {
        interactControlIcon = GetIconForAction( ControlKeys.Interact );
        interactControlString = "";

        // If it's null at this point, get the string for the control instead
        if(!interactControlIcon){
            interactControlString = GetActionString(interactAction);
            controlButtonText.text = interactControlString;
        }

        SetAlertText(alertTextIsActive, interactControlIcon, interactControlString, alertText.text);
    }

    private string GetActionString(InputActionReference action)
    {
        int bindingIndex = action.action.GetBindingIndexForControl(action.action.controls[0]);
        return InputControlPath.ToHumanReadableString(action.action.bindings[bindingIndex].effectivePath,InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
    
    private Sprite GetIconForAction(ControlKeys key)
    {
        if(InputManager.instance.latestInputIsController){
            return ControlsMenu.currentControlButtonSpritesForController[key];
        }
        else{
            return ControlsMenu.currentControlButtonSpritesForKeyboard[key];
        }
    }
}
