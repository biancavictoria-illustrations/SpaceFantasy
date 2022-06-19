using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public class OnAbilityEvent : UnityEvent {}

    public static InputManager instance;

    [HideInInspector] public bool preventInputOverride = false;
    [HideInInspector] public bool isInDialogue = false;
    [HideInInspector] public bool inventoryIsOpen = false;
    [HideInInspector] public bool shopIsOpen = false;
    [HideInInspector] public bool compareItemIsOpen = false;
    [HideInInspector] public bool journalIsOpen = false;
    [HideInInspector] public bool mapIsOpen = false;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public OnAbilityEvent useAccessory;
    [HideInInspector] public OnAbilityEvent useHelmet;
    [HideInInspector] public OnAbilityEvent useLegs;

    [HideInInspector] public bool isInMainMenu = false;

    [SerializeField][FMODUnity.EventRef] private string itemOnCooldownSFX;

    public bool latestInputIsController {get; private set;}

    [HideInInspector] public Vector3 cursorLookDirection;

    private Player player;
    private Vector2 mousePos;

    public Vector2 moveDirection;   // For cursorLookDirection on controller

    public static bool aimAtCursor;

    // TEMP - REMOVE THIS FOR FINAL BUILD
    public DevPanel devPanel;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        // UpdateLatestInputDevice();

        useAccessory = new OnAbilityEvent();
        useHelmet = new OnAbilityEvent();
        useLegs = new OnAbilityEvent();
    }

    void Start()
    {
        if(Player.instance){
            player = Player.instance;
        }

        if(GameManager.instance.currentSceneName == GameManager.TITLE_SCREEN_STRING_NAME){
            isInMainMenu = true;
        }
        else{
            InputActionAsset controls = GetComponent<PlayerInput>().actions;
            // Add STOPPING when you're no longer holding the button
            controls.FindAction("AttackPrimary").canceled += x => OnAttackPrimaryCanceled();
        }

        UserDeviceManager.OnInputDeviceChanged.AddListener(OnInputDeviceChangedEvent);

        if(GameManager.instance.InSceneWithRandomGeneration()){
            FindObjectOfType<ControlsMenu>().SetupControlIconsOnStart();
            AlertTextUI.instance.SetupAlertTextOnStart();
            InGameUIManager.instance.SetupAllItemControlButtons();
        }

        if(!devPanel){
            devPanel = FindObjectOfType<DevPanel>();
        }
    }

    public void OnInputDeviceChangedEvent(InputDevice device)
    {
        if(device == InputDevice.KeyboardMouse){
            latestInputIsController = false;
        }
        else{
            latestInputIsController = true;
        }

        // TEMP
        if(!devPanel){
            devPanel = FindObjectOfType<DevPanel>();
        }
        devPanel?.ToggleInteractabilityOnDeviceChange( !latestInputIsController );
    }

    void Update()
    {
        if(isInMainMenu){
            return;
        }

        Vector2 cursorLookDirection2D;
        if(latestInputIsController || !aimAtCursor){
            cursorLookDirection2D = moveDirection;
        }
        else{
            Vector2 playerPositionOnScreen = (Vector2)Camera.main.WorldToScreenPoint(player.transform.position);    //Get Player's position on the screen
            cursorLookDirection2D = (mousePos - playerPositionOnScreen).normalized;     //Get the vector from the player position to the cursor position 
        }
        Vector3 lookDirectionRelativeToCamera = Camera.main.transform.right * cursorLookDirection2D.x + Camera.main.transform.up * cursorLookDirection2D.y; //Create the look vector in 3D space relative to the camera
        cursorLookDirection = Quaternion.FromToRotation(-Camera.main.transform.forward, Vector3.up) * lookDirectionRelativeToCamera;    //Rotate the look vector to be in terms of world space
    }

    public void SetLookDirectionHorizontal( float value)
    {
        moveDirection.x = value;
    }

    public void SetLookDirectionVertical( float value)
    {
        moveDirection.y = value;
    }

    public bool CanAcceptGameplayInput()
    {
        if(preventInputOverride || isInDialogue || PauseMenu.GameIsPaused || inventoryIsOpen || shopIsOpen || compareItemIsOpen || mapIsOpen || journalIsOpen || isInMainMenu || GameManager.instance.statRerollUIOpen || GameManager.instance.inElevatorAnimation || LoadScreen.instance.sceneIsLoading){
            return false;
        }
        return true;
    }

    public void OnAttackPrimary(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }
        isAttacking = true;

        // Debug.Log(moveDirection);
    }

    public void OnAttackPrimaryCanceled()
    {
        isAttacking = false;
    }

    public void OnInteract(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // If nearby NPC is active and not already talking and has something to say
        else if(NPC.ActiveNPC && !NPC.ActiveNPC.haveTalkedToThisRun){
            DialogueManager.instance.OnNPCInteracted();
        }
        // If nearby NPC is a shopkeeper and you HAVE talked to them already, just open the shop
        else if(NPC.ActiveNPC && NPC.ActiveNPC.SpeakerData().IsShopkeeper()){
            // If we just completed the FIRST or LAST run, don't open Stellan's shop
            if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Stellan && (GameManager.instance.currentRunNumber == 2 || GameManager.instance.epilogueTriggered)){
                return;
            }
            InGameUIManager.instance.OpenNPCShop(NPC.ActiveNPC.SpeakerData());
        }

        // If you're in range of a door, walk through it
        else if(SceneTransitionDoor.ActiveDoor){
            AudioManager.Instance.stopMusic(true);  // So that it doesn't persist into load screen
            AlertTextUI.instance.DisablePrimaryAlert();
            AlertTextUI.instance.DisableSecondaryAlert();
            SceneTransitionDoor.ActiveDoor.ChangeScene();
        }

        // Check if you're in range of a dropped item
        else if(DropTrigger.ActiveGearDrop){
            GeneratedEquipment itemData = DropTrigger.ActiveGearDrop;
            ToggleCompareItemUI(true, itemData);
        }

        else if(OrbyBartenderChat.instance && OrbyBartenderChat.instance.inOrbyRange){
            OrbyBartenderChat.instance.OnOrbyInteracted();
        }

        // If run 1 and you're in range of the captain's log
        else if( SpawnRoomForceFieldUnlockItem.activeForceFieldUnlockItem && !SpawnRoomForceFieldUnlockItem.activeForceFieldUnlockItem.isWeapon ){
            // Drop the force fields to unlock the spawn room
            SpawnRoomForceFieldUnlockItem.activeForceFieldUnlockItem.UnlockForceFieldsOnPickUp();
            
            // Alert the dialogue manager we want to play this dialogue
            DialogueManager.instance.SetCaptainsLogDialogueTriggered(true);
            StartCoroutine(DialogueManager.instance.AutoRunDialogueAfterTime(0.5f));
        }
    }

    public void ToggleCompareItemUI(bool set, GeneratedEquipment item)
    {
        compareItemIsOpen = set;
        InGameUIManager.instance.SetGearSwapUIActive(set, item);
        RunGameTimer(set);
    }

    public void OnCancel(InputValue input)
    {
        // If we're using controller, allow B button to close menus
        if(latestInputIsController && (PauseMenu.GameIsPaused || inventoryIsOpen || shopIsOpen || compareItemIsOpen || GameManager.instance.statRerollUIOpen || mapIsOpen || journalIsOpen)){
            OnPause(input);
        }
    }
    
    public void OnSubmit(InputValue input)
    {
        ProgressDialogueOnInput(input);
    }

    public void OnClick(InputValue input)
    {
        ProgressDialogueOnInput(input);        
    }

    private void ProgressDialogueOnInput(InputValue input)
    {
        // input.isPressed confirms this is only run on KEY DOWN, not both key down AND then also key up
        if(isInDialogue && input.isPressed && !PauseMenu.GameIsPaused && !DialogueManager.instance.dialogueUI.IsMidDialogueLineDisplay){
            DialogueManager.instance.dialogueUI.MarkLineComplete();
        }
    }

    public void OnPause(InputValue input)
    {
        if(isInMainMenu){
            return;
        }

        // If you're currently in a UI menu, CLOSE IT and return
        if(inventoryIsOpen){
            OnToggleInventory(input);
            return;
        }
        else if(shopIsOpen){
            InGameUIManager.instance.CloseNPCShop(NPC.ActiveNPC.SpeakerData(), true);
            return;
        }
        else if(compareItemIsOpen){
            InGameUIManager.instance.gearSwapUI.CloseGearSwapUI();
            return;            
        }
        else if(GameManager.instance.statRerollUIOpen){ // Should this one be here?
            InGameUIManager.instance.statRerollUI.DisableStatRerollUI();
            return;
        }
        else if(mapIsOpen){
            OnToggleMinimap(input);
            return;
        }
        else if(journalIsOpen){
            OnToggleJournal(input);
            return;
        }
        
        if(PauseMenu.GameIsPaused){
            InGameUIManager.instance.pauseMenu.ResumeGame(true);       
        }
        else{
            InGameUIManager.instance.pauseMenu.PauseGame();       
        }
    }

    public void OnToggleJournal(InputValue input)
    {
        if(isInDialogue || PauseMenu.GameIsPaused || shopIsOpen || compareItemIsOpen || inventoryIsOpen || mapIsOpen || isInMainMenu || !PlayerInventory.hasCaptainsLog){
            return;
        }

        journalIsOpen = !journalIsOpen;
        InGameUIManager.instance.journalUI.ToggleJournalActive(journalIsOpen);

        // Deal with timer if we're in a scene with the timer
        if(GameManager.instance.InSceneWithGameTimer()){
            RunGameTimer(!journalIsOpen);
            InGameUIManager.instance.timerUI.SetTimerUIActive(!journalIsOpen);
        }

        if(AlertTextUI.instance.primaryAlertTextIsActive){
            AlertTextUI.instance.TogglePrimaryAlertText(!journalIsOpen);
        }
        if(AlertTextUI.instance.secondaryAlertTextIsActive){
            AlertTextUI.instance.ToggleSecondaryAlertText(!journalIsOpen);
        }
    }

    // I did something dumb and changed the name of this one and thought it was broken so this is a friendly reminder
    // that these names have to be the same as the ones in the control scheme and therefore this one HAS TO BE ToggleMinimap
    // even tho for a sec I thought it would make more sense as "ToggleExpandedMap" instead
    public void OnToggleMinimap(InputValue input)
    {
        if(!GameManager.instance.InSceneWithRandomGeneration() || isInDialogue || PauseMenu.GameIsPaused || shopIsOpen || compareItemIsOpen || inventoryIsOpen || journalIsOpen || !PlayerInventory.hasCaptainsLog){
            return;
        }

        mapIsOpen = !mapIsOpen;
        InGameUIManager.instance.ToggleExpandedMapOverlay(mapIsOpen);

        RunGameTimer(!mapIsOpen);
        InGameUIManager.instance.timerUI.SetTimerUIActive(!mapIsOpen);

        if(AlertTextUI.instance.primaryAlertTextIsActive){
            AlertTextUI.instance.TogglePrimaryAlertText(!mapIsOpen);
        }
        if(AlertTextUI.instance.secondaryAlertTextIsActive){
            AlertTextUI.instance.ToggleSecondaryAlertText(!mapIsOpen);
        }
    }

    public void OnToggleInventory(InputValue input)
    {
        if(isInDialogue || PauseMenu.GameIsPaused || shopIsOpen || compareItemIsOpen || mapIsOpen || journalIsOpen || GameManager.instance.currentSceneName == GameManager.MAIN_HUB_STRING_NAME || isInMainMenu){
            return;
        }
        
        InGameUIManager.instance.SetInventoryUIActive(!InGameUIManager.instance.inventoryIsOpen);
        inventoryIsOpen = !inventoryIsOpen;

        if(GameManager.instance.InSceneWithRandomGeneration())
            InGameUIManager.instance.ToggleMiniMap(!inventoryIsOpen);

        if(GameManager.instance.InSceneWithGameTimer())
            RunGameTimer(!inventoryIsOpen);

        if(AlertTextUI.instance.primaryAlertTextIsActive){
            AlertTextUI.instance.TogglePrimaryAlertText(!inventoryIsOpen);
        }
        if(AlertTextUI.instance.secondaryAlertTextIsActive){
            AlertTextUI.instance.ToggleSecondaryAlertText(!inventoryIsOpen);
        }
    }

    public void OnUseHealthPotion(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        PlayerInventory.instance.UseHealthPotion();
    }

    public void OnAccessoryAbility()
    {
        // If we can't move at all OR if this cooldown is currently active, return (using cooldown status from UI because we want to continue the cooldown if you swapped items)
        if (!CanAcceptGameplayInput()){
            return;
        }

        if(ItemSlotIsActiveOrCooldown(InventoryItemSlot.Accessory)){
            AudioManager.Instance.PlaySFX(itemOnCooldownSFX);
            return;
        }

        useAccessory.Invoke();
    }

    public void OnHelmetAbility()
    {
        // If we can't move at all OR if this cooldown is currently active, return (using cooldown status from UI because we want to continue the cooldown if you swapped items)
        if(!CanAcceptGameplayInput()){
            return;
        }

        if(ItemSlotIsActiveOrCooldown(InventoryItemSlot.Helmet)){
            AudioManager.Instance.PlaySFX(itemOnCooldownSFX);
            return;
        }

        useHelmet.Invoke();
    }

    public void OnBootsAbility()
    {
        // If we can't move at all OR if this cooldown is currently active, return (using cooldown status from UI because we want to continue the cooldown if you swapped items)
        if(!CanAcceptGameplayInput()){
            return;
        }

        if(ItemSlotIsActiveOrCooldown(InventoryItemSlot.Legs)){
            AudioManager.Instance.PlaySFX(itemOnCooldownSFX);
            return;
        }

        useLegs.Invoke();
    }

    private bool ItemSlotIsActiveOrCooldown(InventoryItemSlot itemSlot)
    {
        if(InGameUIManager.instance.GetCooldownUIFromSlot(itemSlot).isActive){
            return true;
        }
        else if( PlayerInventory.instance.ItemSlotIsFull(itemSlot) && ((NonWeaponItem)PlayerInventory.instance.gear[itemSlot]).durationRoutine != null ){
            return true;
        }
        else{
            return false;
        }
    }

    public void OnPoint(InputValue input)
    {
        mousePos = input.Get<Vector2>();
    }

    public void RunGameTimer(bool runTimer, bool setTimerUIActive = true)
    {
        InGameUIManager.instance.timerUI.SetTimerUIActive(setTimerUIActive);

        if(GameManager.instance.gameTimer.timerHasStartedForRun)
            GameManager.instance.gameTimer.runTimer = runTimer;
    }

    public void ToggleShopOpenStatus(bool set)
    {
        shopIsOpen = set;
        RunGameTimer(!set);
    }

    public void ToggleDialogueOpenStatus(bool set)
    {
        isInDialogue = set;

        if(GameManager.instance.InSceneWithGameTimer()){
            RunGameTimer(!set, !set);
        }
    }
}
