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
    private bool interactAlertIsActive = false;

    private Sprite openInventoryControlIcon;
    private string openInventoryControlString;
    private bool openInventoryAlertIsActive = false;

    private Sprite openJournalControlIcon;
    private string openJournalControlString;
    private bool openJournalAlertIsActive = false;
    
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference toggleInventoryAction;
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
        if(GameManager.instance.InSceneWithRandomGeneration()){
            FindObjectOfType<FloorGenerator>().OnGenerationComplete.AddListener(StartOnGenerationComplete);
        }        
        else{
            StartOnGenerationComplete();
        }
    }

    private void StartOnGenerationComplete()
    {
        alertTextIsActive = false;
        UpdateAlertText();
    }

    // TODO: DO THIS NOT IN UPDATE!!!
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
        interactAlertIsActive = false;
        openInventoryAlertIsActive = false;
        openJournalAlertIsActive = false;
    }

    public IEnumerator RemoveAlertAfterSeconds(float timeToWait = 2f)
    {
        yield return new WaitForSeconds(timeToWait);
        DisableAlert();   
    }

    public void EnableOpenJournalAlert()
    {
        SetAlertText(true, openJournalControlIcon, openJournalControlString, "OPEN CAPTAIN'S LOG");
        alertTextIsActive = true;
        openJournalAlertIsActive = true;
    }

    #region Interact Alerts
        public void EnableInteractAlert()
        {
            SetAlertText(true, interactControlIcon, interactControlString, "INTERACT");
            alertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnableShopAlert()
        {
            SetAlertText(true, interactControlIcon, interactControlString, "SHOP");
            alertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnableProceedDoorAlert()
        {
            SetAlertText(true, interactControlIcon, interactControlString, "PROCEED");
            alertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnablePickupAlert()
        {
            SetAlertText(true, interactControlIcon, interactControlString, "PICK UP");
            alertTextIsActive = true;
            interactAlertIsActive = true;
        }

        public void EnableItemExamineAlert()
        {
            SetAlertText(true, interactControlIcon, interactControlString, "EXAMINE");
            alertTextIsActive = true;
            interactAlertIsActive = true;
        }
    #endregion

    #region Inventory Alerts
        public void EnableOpenInventoryAlert()
        {
            SetAlertText(true, openInventoryControlIcon, openInventoryControlString, "OPEN INVENTORY");
            alertTextIsActive = true;
            openInventoryAlertIsActive = true;
        }

        public void EnableViewStatsAlert()
        {
            SetAlertText(true, openInventoryControlIcon, openInventoryControlString, "VIEW STATS");
            alertTextIsActive = true;
            openInventoryAlertIsActive = true;
        }
    #endregion

    // If showing alerts for more than just interact, UPDATE this to include those as well
    public void UpdateAlertText()
    {
        interactControlIcon = GetIconForAction( ControlKeys.Interact );
        interactControlString = "";

        openInventoryControlIcon = GetIconForAction(ControlKeys.ToggleInventory);
        openInventoryControlString = "";

        openJournalControlIcon = GetIconForAction(ControlKeys.ToggleJournal);
        openJournalControlString = "";

        // If it's null at this point, get the string for the control instead
        if(!interactControlIcon){
            interactControlString = GetActionString(interactAction);
            controlButtonText.text = interactControlString;
        }
        if(!openInventoryControlIcon){
            openInventoryControlString = GetActionString(toggleInventoryAction);
            controlButtonText.text = openInventoryControlString;
        }
        if(!openJournalControlIcon){
            openJournalControlString = GetActionString(toggleJournalAction);
            controlButtonText.text = openJournalControlString;
        }

        if(openInventoryAlertIsActive){
            SetAlertText(alertTextIsActive, openInventoryControlIcon, openInventoryControlString, alertText.text);
        }
        else if(interactAlertIsActive){
            SetAlertText(alertTextIsActive, interactControlIcon, interactControlString, alertText.text);
        }
        else if(openJournalAlertIsActive){
            SetAlertText(alertTextIsActive, openJournalControlIcon, openJournalControlString, alertText.text);
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
