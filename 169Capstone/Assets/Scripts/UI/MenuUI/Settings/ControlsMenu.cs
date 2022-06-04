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
    ToggleMinimap,
    Interact,
    Pause,
    ToggleInventory,
    UseHealthPotion,
    AccessoryAbility,
    HelmetAbility,
    BootsAbility,
    ToggleJournal,

    enumSize
}

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private HashSet<ControlRebindButton> buttons = new HashSet<ControlRebindButton>();
    [SerializeField] private SaveLoadControls saveLoadControls;

    [SerializeField] private Button applyButton;
    [SerializeField] private Button resetButton;

    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private DeviceButtonSpritesObject spritesForKeyboardControls;
    [SerializeField] private DeviceButtonSpritesObject spritesForXboxControls;

    public static Dictionary<ControlKeys, Sprite> currentControlButtonSpritesForKeyboard {get; private set;}
    public static Dictionary<ControlKeys, Sprite> currentControlButtonSpritesForController {get; private set;}

    [HideInInspector] public bool currentlyRebinding = false;

    #region Keybinding Buttons
        [SerializeField] private ControlRebindButton moveUp;
        [SerializeField] private ControlRebindButton moveDown;
        [SerializeField] private ControlRebindButton moveLeft;
        [SerializeField] private ControlRebindButton moveRight;
        [SerializeField] private ControlRebindButton jump;
        
        [SerializeField] private ControlRebindButton primaryAttack;
        [SerializeField] private ControlRebindButton accessoryAbility;
        [SerializeField] private ControlRebindButton helmetAbility;
        [SerializeField] private ControlRebindButton bootsAbility;

        [SerializeField] private ControlRebindButton useHealthPotion;
        [SerializeField] private ControlRebindButton interact;
        [SerializeField] private ControlRebindButton toggleInventory;
        [SerializeField] private ControlRebindButton toggleJournal;
        [SerializeField] private ControlRebindButton toggleMinimap;
        [SerializeField] private ControlRebindButton pause;
    #endregion

    void Start()
    {
        // If we're in a scene with random generation, the InputManager calls this instead
        if(!GameManager.instance.InSceneWithRandomGeneration()){
            SetupControlIconsOnStart();
        }
    }

    public void OnInputDeviceChangedEvent(InputDevice device)
    {
        if(!currentlyRebinding){
            UpdateAllButtonText();
        }
    }

    public void SetupControlIconsOnStart()
    {
        UserDeviceManager.OnInputDeviceChanged.AddListener(OnInputDeviceChangedEvent);

        currentControlButtonSpritesForKeyboard = new Dictionary<ControlKeys, Sprite>();
        currentControlButtonSpritesForController = new Dictionary<ControlKeys, Sprite>();

        buttons.Add(moveUp);
        buttons.Add(moveDown);
        buttons.Add(moveLeft);
        buttons.Add(moveRight);
        buttons.Add(jump);
        buttons.Add(primaryAttack);
        buttons.Add(toggleMinimap);
        buttons.Add(accessoryAbility);
        buttons.Add(helmetAbility);
        buttons.Add(bootsAbility);
        buttons.Add(useHealthPotion);
        buttons.Add(interact);
        buttons.Add(toggleInventory);
        buttons.Add(toggleJournal);
        buttons.Add(pause);

        saveLoadControls.controls = controls;

        if(!PlayerPrefs.HasKey("ControlOverrides")){
            // Save default as prefs
            saveLoadControls.StoreControlOverrides();
        }
        else{   // Load existing prefs
            SetControlsToSavedValues();
        }

        if(AlertTextUI.instance){
            AlertTextUI.instance.UpdateAlertText();
        }
    }

    public void SetBottomButtonsInteractable(bool set)
    {
        applyButton.interactable = set;
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
        currentlyRebinding = true;

        if((int)key > 14){
            Debug.LogError("Cannot rebind setting: " + key.ToString());
            return;
        }

        controls.Disable();
        SetBottomButtonsInteractable(false);

        GetButtonFromKey(key).SetToCurrentlyRebinding();

        InputAction action = GetAction(key);

        bool canceled = false;

        // If latest input is controller, only rebind this for that control scheme
        if( InputManager.instance.latestInputIsController ){
            rebindingOperation = action.PerformInteractiveRebinding()
                .WithBindingGroup("Gamepad")
                .WithControlsExcluding("Mouse")
                .WithControlsExcluding("<Keyboard>")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete(key,action,canceled))
                .Start();
        }   // Else only rebind for keyboard scheme
        else{
            rebindingOperation = action.PerformInteractiveRebinding()
                .WithBindingGroup("Keyboard")
                .WithControlsExcluding("Mouse")
                .WithControlsExcluding("<Gamepad>")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete(key,action,canceled))
                .Start();
        }        
    }

    private void RebindComplete(ControlKeys key, InputAction action, bool canceled=false)
    {
        int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);

        // If an invalid binding is found (such as a duplicate), wait for new input instead
        if(bindingIndex < 0){
            Debug.LogWarning("Invalid binding found. Binding index: " + bindingIndex);
            CleanUpAction();
            StartRebinding(key);
            return;
        }
        // Once we have a valid binding index, check for duplicates
        if(IsDuplicateBinding(key, action, bindingIndex)){
            // TODO: Give UI feedback that it's a duplicate!

            action.RemoveBindingOverride(bindingIndex);
            CleanUpAction();
            StartRebinding(key);
            return;
        }

        SetButtonIcon(key, action, bindingIndex);

        CleanUpAction();
        controls.Enable();
        SetBottomButtonsInteractable(true);

        currentlyRebinding = false;
    }

    private bool IsDuplicateBinding(ControlKeys key, InputAction action, int bindingIndex)
    {  
        InputBinding newBinding = action.bindings[bindingIndex];
        foreach( InputBinding binding in action.actionMap.bindings ){
            if(binding.action == newBinding.action){
                continue;
            }
            if(binding.effectivePath == newBinding.effectivePath){
                Debug.Log("Duplicate binding found: " + newBinding.effectivePath);
                return true;
            }
        }
        return false;
    }


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

        // Get the button on the screen
        ControlRebindButton button = GetButtonFromKey(key);

        // Get the string representing this control button
        string controlName = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        Sprite buttonSprite = null;

        // Get the sprite if controller
        if(InputManager.instance.latestInputIsController){
            // Set the value in the dictionary to store this current control sprite so we don't have to find it from the scriptable object it next time
            currentControlButtonSpritesForController[key] = spritesForXboxControls.GetSprite(controlName);
            buttonSprite = currentControlButtonSpritesForController[key];
        }
        else{   // Get the sprite if keyboard
            currentControlButtonSpritesForKeyboard[key] = spritesForKeyboardControls.GetSprite(controlName);
            buttonSprite = currentControlButtonSpritesForKeyboard[key];
        }

        if( buttonSprite ){
            // If there IS an icon, deactivate the text and set the sprite
            button.buttonText.gameObject.SetActive(false);
            button.SetIconSprite(buttonSprite);
        }
        else{
            // If not, set the image back to the default UI sprite, activate button text, and set the text to the name of the control
            button.buttonText.gameObject.SetActive(true);
            button.SetIconToDefault();

            button.buttonText.text = controlName;
        }
    }

    private void UpdateAllButtonText()
    {
        foreach(ControlRebindButton b in buttons){
            SetButtonIcon(b.ControlKey());
        }
    }

    public void ApplyControlsChange()
    {
        saveLoadControls.StoreControlOverrides();
        
        UpdateControlAlertUI();

        SetControlPanelActive(false);
    }

    public void SetControlsToSavedValues()
    {
        // Load from player prefs
        saveLoadControls.LoadControlOverrides();
        UpdateAllButtonText();
    }

    public void UpdateControlAlertUI()
    {
        // Update control UI elsewhere
        AlertTextUI.instance.UpdateAlertText();
        InGameUIManager.instance.UpdateAllItemControlButtons();
    }

    public void ResetControlsToDefault()
    {
        Debug.Log("Resetting controls to default values...");

        foreach(InputActionMap map in controls.actionMaps){
            map.RemoveAllBindingOverrides();
        }

        saveLoadControls.StoreControlOverrides();
        
        UpdateControlAlertUI();

        UpdateAllButtonText();
    }

    public void SetControlPanelActive(bool set)
    {
        if(!pauseMenu){
            Debug.LogWarning("No pause menu found (don't call SetControlPanelActive on Main Menu)");
            return;
        }

        pauseMenu.controlsMenuPanel.SetActive(set);
        pauseMenu.pauseMenuPanel.SetActive(!set);

        if(set){
            applyButton.Select();
        }
        else{
            pauseMenu.continueButton.Select();
        }
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
            case ControlKeys.ToggleMinimap:
                return toggleMinimap;
            
            case ControlKeys.Interact:
                return interact;
            case ControlKeys.Pause:
                return pause;
            case ControlKeys.ToggleInventory:
                return toggleInventory;
            case ControlKeys.ToggleJournal:
                return toggleJournal;

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