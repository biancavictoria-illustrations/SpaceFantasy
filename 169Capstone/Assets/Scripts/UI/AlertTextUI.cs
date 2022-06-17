using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AlertTextUI : MonoBehaviour
{
    public static AlertTextUI instance;

    [SerializeField] private GameObject primaryAlert;
    [SerializeField] private Image primaryAlertControlButtonIcon;
    [SerializeField] private TMP_Text primaryAlertControlButtonText;
    [SerializeField] private TMP_Text primaryAlertText;

    [SerializeField] private GameObject secondaryAlert;
    [SerializeField] private Image secondaryAlertControlButtonIcon;
    [SerializeField] private TMP_Text secondaryAlertControlButtonText;
    [SerializeField] private TMP_Text secondaryAlertText;

    [HideInInspector] public bool primaryAlertTextIsActive = false;
    [HideInInspector] public bool secondaryAlertTextIsActive = false;

    private Sprite interactControlIcon;
    private string interactControlString;
    private bool interactAlertIsActive = false;

    private Sprite openInventoryControlIcon;
    private string openInventoryControlString;
    private bool openInventoryAlertIsActive = false;

    private Sprite openMapControlIcon;
    private string openMapControlString;
    private bool openMapAlertIsActive = false;

    private Sprite openJournalControlIcon;
    private string openJournalControlString;
    private bool openJournalAlertIsActive = false;
    
    // Primary Alerts
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference toggleInventoryAction;
    [SerializeField] private InputActionReference toggleMapAction;

    // Secondary Alerts (only journal)
    [SerializeField] private InputActionReference toggleJournalAction;

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
        // Called in InputManager otherwise
        if(!GameManager.instance.InSceneWithRandomGeneration()){
            SetupAlertTextOnStart();
        }
    }

    public void SetupAlertTextOnStart()
    {
        primaryAlertTextIsActive = false;
        secondaryAlertTextIsActive = false;

        UserDeviceManager.OnInputDeviceChanged.AddListener(OnInputDeviceChangedEvent);

        UpdateAlertText();
    }

    public void OnInputDeviceChangedEvent(InputDevice device)
    {
        UpdateAlertText();
    }

    #region Set Alert Values
        private void SetPrimaryAlertText(bool set, Sprite _icon=null, string _text="", string newAlertDescription="")
        {
            if(!primaryAlert){
                return;
            }
            
            TogglePrimaryAlertText(set);

            if(_icon){
                primaryAlertControlButtonIcon.gameObject.SetActive(true);
                primaryAlertControlButtonText.gameObject.SetActive(false);

                primaryAlertControlButtonIcon.sprite = _icon;
                primaryAlertControlButtonIcon.preserveAspect = true;
            }
            else{
                primaryAlertControlButtonIcon.gameObject.SetActive(false);
                primaryAlertControlButtonText.gameObject.SetActive(true);
                
                primaryAlertControlButtonText.text = _text;
            }

            primaryAlertText.text = newAlertDescription;
        }

        private void SetSecondaryAlertText(bool set, Sprite _icon=null, string _text="", string newAlertDescription="")
        {
            if(!secondaryAlert){
                return;
            }
            
            ToggleSecondaryAlertText(set);

            if(_icon){
                secondaryAlertControlButtonIcon.gameObject.SetActive(true);
                secondaryAlertControlButtonText.gameObject.SetActive(false);

                secondaryAlertControlButtonIcon.sprite = _icon;
                secondaryAlertControlButtonIcon.preserveAspect = true;
            }
            else{
                secondaryAlertControlButtonIcon.gameObject.SetActive(false);
                secondaryAlertControlButtonText.gameObject.SetActive(true);
                
                secondaryAlertControlButtonText.text = _text;
            }

            secondaryAlertText.text = newAlertDescription;
        }
    #endregion

    #region Toggles
        // For temporarily toggling in pause menu and other screens like that (if necessary)
        public void TogglePrimaryAlertText(bool set)
        {
            if(primaryAlert){
                primaryAlert.SetActive(set);
            }        
        }

        public void ToggleSecondaryAlertText(bool set)
        {
            if(secondaryAlert){
                secondaryAlert.SetActive(set);
            }        
        }

        // Called when you leave a trigger
        public void DisablePrimaryAlert()
        {
            SetPrimaryAlertText(false);
            primaryAlertTextIsActive = false;
            interactAlertIsActive = false;
            openInventoryAlertIsActive = false;
            openMapAlertIsActive = false;
        }

        public void DisableSecondaryAlert()
        {
            SetSecondaryAlertText(false);
            secondaryAlertTextIsActive = false;
            openJournalAlertIsActive = false;
        }

        public IEnumerator RemovePrimaryAlertAfterSeconds(float timeToWait = 2f)
        {
            yield return new WaitForSeconds(timeToWait);
            DisablePrimaryAlert();   
        }

        public IEnumerator RemoveSecondaryAlertAfterSeconds(float timeToWait = 4f)
        {
            yield return new WaitForSeconds(timeToWait);
            DisableSecondaryAlert();
        }
    #endregion

    #region Journal Alerts
        // sometimes they don't work after dialogue... (it's for shopkeepers cuz their shops open........)
        public void EnableOpenJournalAlert()
        {
            SetSecondaryAlertText(true, openJournalControlIcon, openJournalControlString, "OPEN CAPTAIN'S LOG");
            secondaryAlertTextIsActive = true;
            openJournalAlertIsActive = true;

            AudioManager.Instance.PlaySFX(AudioManager.SFX.CaptainsLogAlert);
        }

        public void EnableJournalUpdatedAlert()
        {
            SetSecondaryAlertText(true, openJournalControlIcon, openJournalControlString, "CAPTAIN'S LOG UPDATED");
            secondaryAlertTextIsActive = true;
            openJournalAlertIsActive = true;
            
            AudioManager.Instance.PlaySFX(AudioManager.SFX.CaptainsLogAlert);
        }
    #endregion

    #region Interact Alerts
        public void EnableInteractAlert()
        {
            SetPrimaryAlertText(true, interactControlIcon, interactControlString, "INTERACT");
            primaryAlertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnableShopAlert()
        {
            SetPrimaryAlertText(true, interactControlIcon, interactControlString, "SHOP");
            primaryAlertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnableContinueDoorAlert()
        {
            SetPrimaryAlertText(true, interactControlIcon, interactControlString, "CONTINUE");
            primaryAlertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnableLeaveDoorAlert()
        {
            SetPrimaryAlertText(true, interactControlIcon, interactControlString, "LEAVE");
            primaryAlertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnablePickupAlert()
        {
            SetPrimaryAlertText(true, interactControlIcon, interactControlString, "PICK UP");
            primaryAlertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnableItemExamineAlert()
        {
            SetPrimaryAlertText(true, interactControlIcon, interactControlString, "EXAMINE");
            primaryAlertTextIsActive = true;
            interactAlertIsActive = true;
        }
    #endregion

    #region Inventory Alerts
        public void EnableOpenInventoryAlert()
        {
            SetPrimaryAlertText(true, openInventoryControlIcon, openInventoryControlString, "OPEN INVENTORY");
            primaryAlertTextIsActive = true;
            openInventoryAlertIsActive = true;
        }

        public void EnableViewStatsAlert()
        {
            SetPrimaryAlertText(true, openInventoryControlIcon, openInventoryControlString, "VIEW STATS");
            primaryAlertTextIsActive = true;
            openInventoryAlertIsActive = true;
        }
    #endregion

    public void EnableViewMapAlert()
    {
        SetPrimaryAlertText(true, openMapControlIcon, openMapControlString, "VIEW MAP");
        primaryAlertTextIsActive = true;
        openMapAlertIsActive = true;
    }

    // If showing alerts for more than just interact, UPDATE this to include those as well
    public void UpdateAlertText()
    {
        interactControlIcon = GetIconForAction( ControlKeys.Interact );
        interactControlString = "";

        openInventoryControlIcon = GetIconForAction(ControlKeys.ToggleInventory);
        openInventoryControlString = "";

        openMapControlIcon = GetIconForAction(ControlKeys.ToggleMinimap);
        openMapControlString = "";

        openJournalControlIcon = GetIconForAction(ControlKeys.ToggleJournal);
        openJournalControlString = "";

        // If it's null at this point, get the string for the control instead
        if(!interactControlIcon){
            interactControlString = GetActionString(interactAction);
            primaryAlertControlButtonText.text = interactControlString;
        }
        if(!openInventoryControlIcon){
            openInventoryControlString = GetActionString(toggleInventoryAction);
            primaryAlertControlButtonText.text = openInventoryControlString;
        }
        if(!openMapControlIcon){
            openMapControlString = GetActionString(toggleMapAction);
            primaryAlertControlButtonText.text = openMapControlString;
        }
        if(!openJournalControlIcon){
            openJournalControlString = GetActionString(toggleJournalAction);
            secondaryAlertControlButtonText.text = openJournalControlString;
        }

        // Primary Alerts
        if(openInventoryAlertIsActive){
            SetPrimaryAlertText(primaryAlertTextIsActive, openInventoryControlIcon, openInventoryControlString, primaryAlertText.text);
        }
        else if(interactAlertIsActive){
            SetPrimaryAlertText(primaryAlertTextIsActive, interactControlIcon, interactControlString, primaryAlertText.text);
        }
        else if(openMapAlertIsActive){
            SetPrimaryAlertText(primaryAlertTextIsActive, openMapControlIcon, openMapControlString, primaryAlertText.text);
        }
        
        // Secondary Alerts
        if(openJournalAlertIsActive){
            SetSecondaryAlertText(secondaryAlertTextIsActive, openJournalControlIcon, openJournalControlString, secondaryAlertText.text);
        }
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
