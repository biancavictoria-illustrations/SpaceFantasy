using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public enum ControlKeys
{
    // Keybindings
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Jump,
    AttackPrimary,
    AttackSecondary,
    Interact,
    Pause,
    ToggleInventory,
    UseHealthPotion,
    AccessoryAbility,
    HelmetAbility,
    BootsAbility,

    enumSize
}

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private HashSet<ControlRebindButton> buttons = new HashSet<ControlRebindButton>();
    [SerializeField] private SaveLoadControls saveLoadControls;

    [SerializeField] private Button applyButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button resetButton;

    // Keybinding buttons
    [SerializeField] private ControlRebindButton moveUp;
    [SerializeField] private ControlRebindButton moveDown;
    [SerializeField] private ControlRebindButton moveLeft;
    [SerializeField] private ControlRebindButton moveRight;
    [SerializeField] private ControlRebindButton jump;
    
    [SerializeField] private ControlRebindButton primaryAttack;
    [SerializeField] private ControlRebindButton secondaryAttack;
    [SerializeField] private ControlRebindButton accessoryAbility;
    [SerializeField] private ControlRebindButton helmetAbility;
    [SerializeField] private ControlRebindButton bootsAbility;

    [SerializeField] private ControlRebindButton useHealthPotion;
    [SerializeField] private ControlRebindButton interact;
    [SerializeField] private ControlRebindButton toggleInventory;
    [SerializeField] private ControlRebindButton pause;


    public void Start()
    {
        buttons.Add(moveUp);
        buttons.Add(moveDown);
        buttons.Add(moveLeft);
        buttons.Add(moveRight);
        buttons.Add(jump);
        buttons.Add(primaryAttack);
        buttons.Add(secondaryAttack);
        buttons.Add(accessoryAbility);
        buttons.Add(helmetAbility);
        buttons.Add(bootsAbility);
        buttons.Add(useHealthPotion);
        buttons.Add(interact);
        buttons.Add(toggleInventory);
        buttons.Add(pause);

        saveLoadControls.controls = controls;

        if(!PlayerPrefs.HasKey("ControlOverrides")){
            // Save default as prefs
            saveLoadControls.StoreControlOverrides();
        }
        else{   // Load existing prefs
            SetControlsToSavedValues();
        }

        AlertTextUI.instance.UpdateAlertText();
    }

    public void SetBottomButtonsInteractable(bool set)
    {
        applyButton.interactable = set;
        cancelButton.interactable = set;
        resetButton.interactable = set;
    }

    private void CleanUpAction()
    {
        if(rebindingOperation != null){
            rebindingOperation.Dispose();
        }
        rebindingOperation = null;
    }

    private InputAction GetAction(ControlKeys key)
    {
        return controls.FindAction(key.ToString());
    }

    // Called when you click a button
    public void Rebind(ControlRebindButton b)
    {
        StartRebinding(b.ControlKey());
    }

    private void StartRebinding(ControlKeys key)
    {
        if((int)key > 13){
            Debug.LogError("Cannot rebind setting: " + key.ToString());
            return;
        }

        controls.Disable();
        SetBottomButtonsInteractable(false);

        GetButtonFromKey(key).buttonText.text = "...";
        InputAction action = GetAction(key);

        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete(key,action))
            .Start();
    }

    private void RebindComplete(ControlKeys key, InputAction action)
    {
        int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);

        // If an invalid binding is found (such as a duplicate), wait for new input instead
        // TODO: Alternatively, could catch duplicates and instead swap them (putting nothing "-" in the new one)?
        // this doesn't consistently catch duplicates
        if(bindingIndex < 0){
            // TODO: Give UI feedback that this is invalid!!!

            Debug.LogWarning("Invalid binding found. Binding index: " + bindingIndex);
            CleanUpAction();
            StartRebinding(key);
            return;
        }

        SetButtonIcon(key, action, bindingIndex);

        CleanUpAction();
        controls.Enable();
        SetBottomButtonsInteractable(true);
    }

    // TODO: Switch to ICONS instead of text (text if no icon found presumably)
    private void SetButtonIcon(ControlKeys key, InputAction action = null, int bindingIndex = -1)
    {
        if(action == null){
            action = GetAction(key);
            bindingIndex = action.GetBindingIndexForControl(action.controls[0]);
        }
        // If the binding index is STILL < 0, throw an error
        if(bindingIndex < 0){
            Debug.LogError("Binding index out of bounds! Binding index: " + bindingIndex);
            return;
        }

        GetButtonFromKey(key).buttonText.text = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        // TODO: do we have to deal with setting different device data or do the schemes handle that?
    }

    // TODO: Call this if you switch input device
    private void UpdateAllButtonText()
    {
        foreach(ControlRebindButton b in buttons){
            SetButtonIcon(b.ControlKey());
        }
    }

    public void ApplyControlsChange()
    {
        Debug.Log("Applying controls changes...");
        saveLoadControls.StoreControlOverrides();
        
        AlertTextUI.instance.UpdateAlertText();
    }

    public void CancelControlsChange()
    {
        Debug.Log("No controls changes applied.");
        SetControlsToSavedValues();
    }

    public void SetControlsToSavedValues()
    {
        // Load from player prefs
        saveLoadControls.LoadControlOverrides();
        UpdateAllButtonText();
    }

    public void ResetControlsToDefault()
    {
        Debug.Log("Resetting controls to default values...");

        foreach(InputActionMap map in controls.actionMaps){
            map.RemoveAllBindingOverrides();
        }
        UpdateAllButtonText();
        // could make it so that this is permanent and have a "are you sure?" popup, and then add the line below to set it
        // PlayerPrefs.DeleteKey("ControlOverrides");
    }

    private ControlRebindButton GetButtonFromKey(ControlKeys key)
    {
        switch(key){
            case ControlKeys.MoveUp:
                return moveUp;
            case ControlKeys.MoveLeft:
                return moveLeft;
            case ControlKeys.MoveRight:
                return moveRight;
            case ControlKeys.MoveDown:
                return moveDown;
            case ControlKeys.Jump:
                return jump;

            case ControlKeys.AttackPrimary:
                return primaryAttack;
            case ControlKeys.AttackSecondary:
                return secondaryAttack;
            
            case ControlKeys.Interact:
                return interact;
            case ControlKeys.Pause:
                return pause;
            case ControlKeys.ToggleInventory:
                return toggleInventory;

            case ControlKeys.UseHealthPotion:
                return useHealthPotion;
            case ControlKeys.AccessoryAbility:
                return accessoryAbility;
            case ControlKeys.HelmetAbility:
                return helmetAbility;
            case ControlKeys.BootsAbility:
                return bootsAbility;

            default:
                Debug.LogError("No button found for key: " + key.ToString());
                return null;
        }
    }
}


/*
    - If you try to set a duplicate -> that doesn't work -> set something else -> cancel, it saves it for some reason
        > doesn't consistently catch duplicates

    - sometimes cancel just still doesn't work normally
*/