using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [HideInInspector] public bool isInDialogue = false;
    [HideInInspector] public bool inventoryIsOpen = false;
    [HideInInspector] public bool shopIsOpen = false;
    [HideInInspector] public bool compareItemIsOpen = false;
    [HideInInspector] public bool journalIsOpen = false;
    [HideInInspector] public bool mapIsOpen = false;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool useAccessory = false;
    [HideInInspector] public bool useHead = false;
    [HideInInspector] public bool useLegs = false;

    [HideInInspector] public bool isInMainMenu = false;

    public bool latestInputIsController {get; private set;}
    public InputDevice currentDevice {get; private set;}

    [HideInInspector] public Vector3 cursorLookDirection;

    private Player player;
    private Vector2 mousePos;

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        latestInputIsController = false;
        currentDevice = InputDevice.KeyboardMouse;
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
    }

    void Update()
    {
        if(isInMainMenu){
            return;
        }

        Vector2 playerPositionOnScreen = (Vector2)Camera.main.WorldToScreenPoint(player.transform.position); //Get Player's position on the screen
        Vector2 cursorLookDirection2D = (mousePos - playerPositionOnScreen).normalized;                      //Get the vector from the player position to the cursor position 
        Vector3 lookDirectionRelativeToCamera = Camera.main.transform.right * cursorLookDirection2D.x + Camera.main.transform.up * cursorLookDirection2D.y; //Create the look vector in 3D space relative to the camera
        cursorLookDirection = Quaternion.FromToRotation(-Camera.main.transform.forward, Vector3.up) * lookDirectionRelativeToCamera;                        //Rotate the look vector to be in terms of world space
    }

    public void UpdateLatestInputDevice()
    {
        if((int)UserDeviceManager.currentControlDevice == 0){
            latestInputIsController = false;
            currentDevice = InputDevice.KeyboardMouse;
            Debug.Log("input device is now keyboard/mouse");
        }
        else{
            latestInputIsController = true;

            // InputDevice.
            // UserDeviceManager.currentControlDevice

            Debug.Log("input device is now a controller");
        }
    }

    public bool CanAcceptGameplayInput()
    {
        if(isInDialogue || PauseMenu.GameIsPaused || inventoryIsOpen || shopIsOpen || compareItemIsOpen || mapIsOpen || journalIsOpen || isInMainMenu || GameManager.instance.statRerollUIOpen || GameManager.instance.inElevatorAnimation){
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

        // animator.SetBool("IsAttacking", true);
        // Debug.Log(animator.GetLayerIndex("Slashing"));
        // gameObject.GetComponent<Animator>().Rebind();
        // animator.SetLayerWeight(1, 1);
    }

    public void OnAttackPrimaryCanceled()
    {
        isAttacking = false;

        // animator.SetBool("IsAttacking", false);
        // animator.SetLayerWeight(1, 0);
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
            // If we just completed the FIRST run, don't open Stellan's shop
            if(NPC.ActiveNPC.SpeakerData().SpeakerID() == SpeakerID.Stellan && GameManager.instance.currentRunNumber == 2){
                return;
            }
            InGameUIManager.instance.OpenNPCShop(NPC.ActiveNPC.SpeakerData());
        }

        // If you're in range of a door, walk through it
        else if(SceneTransitionDoor.ActiveDoor){
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
    }

    public void ToggleCompareItemUI(bool set, GeneratedEquipment item)
    {
        compareItemIsOpen = set;
        InGameUIManager.instance.SetGearSwapUIActive(set, item);
        RunGameTimer(set);
    }

    // If you're in dialogue, click anywhere to progress
    public void OnClick(InputValue input)
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
        else if(GameManager.instance.statRerollUIOpen){ // Should this one be here? if so probably also death UI. i'm thinking neither tho?
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
            InGameUIManager.instance.pauseMenu.ResumeGame();

            if(GameManager.instance.currentSceneName != GameManager.MAIN_HUB_STRING_NAME){
                RunGameTimer(true);
            }            
        }
        else{
            InGameUIManager.instance.pauseMenu.PauseGame();

            if(GameManager.instance.currentSceneName != GameManager.MAIN_HUB_STRING_NAME){
                RunGameTimer(false);
            }            
        }
    }

    public void OnToggleJournal(InputValue input)
    {
        if(isInDialogue || PauseMenu.GameIsPaused || shopIsOpen || compareItemIsOpen || inventoryIsOpen || mapIsOpen || isInMainMenu){
            return;
        }

        InGameUIManager.instance.journalUI.ToggleJournalActive(!journalIsOpen);
        journalIsOpen = !journalIsOpen;

        if(GameManager.instance.currentSceneName != GameManager.MAIN_HUB_STRING_NAME){
            RunGameTimer(!journalIsOpen);
            InGameUIManager.instance.timerUI.SetTimerUIActive(!journalIsOpen);
        }

        if(AlertTextUI.instance.alertTextIsActive){
            AlertTextUI.instance.ToggleAlertText(!journalIsOpen);
        }
    }

    public void OnToggleMinimap(InputValue input)
    {
        if(!GameManager.instance.InSceneWithRandomGeneration() || isInDialogue || PauseMenu.GameIsPaused || shopIsOpen || compareItemIsOpen || inventoryIsOpen || journalIsOpen){
            return;
        }

        mapIsOpen = !mapIsOpen;
        InGameUIManager.instance.ToggleExpandedMapOverlay(mapIsOpen);

        RunGameTimer(!mapIsOpen);
        InGameUIManager.instance.timerUI.SetTimerUIActive(!mapIsOpen);

        if(AlertTextUI.instance.alertTextIsActive){
            AlertTextUI.instance.ToggleAlertText(!mapIsOpen);
        }
    }

    public void OnToggleInventory(InputValue input)
    {
        if(isInDialogue || PauseMenu.GameIsPaused || shopIsOpen || compareItemIsOpen || mapIsOpen || journalIsOpen || GameManager.instance.currentSceneName == GameManager.MAIN_HUB_STRING_NAME || isInMainMenu){
            return;
        }
        
        InGameUIManager.instance.SetInventoryUIActive(!InGameUIManager.instance.inventoryIsOpen);
        inventoryIsOpen = !inventoryIsOpen;

        RunGameTimer(!inventoryIsOpen);
        if(AlertTextUI.instance.alertTextIsActive){
            AlertTextUI.instance.ToggleAlertText(!inventoryIsOpen);
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
        if (!CanAcceptGameplayInput())
        {
            return;
        }

        useAccessory = true;
    }

    public void OnAccessoryAbilityCanceled()
    {
        useAccessory = false;
    }

    public void OnHelmetAbility()
    {
        if(!CanAcceptGameplayInput())
        {
            return;
        }

        useHead = true;
    }

    public void OnBootsAbility()
    {
        if(!CanAcceptGameplayInput())
        {
            return;
        }

        useLegs = true;
    }

    public void OnPoint(InputValue input)
    {
        mousePos = input.Get<Vector2>();
    }

    public void RunGameTimer(bool set, bool setTimerUIActive = true)
    {
        InGameUIManager.instance.timerUI.SetTimerUIActive(setTimerUIActive);
        GameManager.instance.gameTimer.runTimer = set;
    }

    public void ToggleShopOpenStatus(bool set)
    {
        shopIsOpen = set;
        RunGameTimer(!set);
    }

    public void ToggleDialogueOpenStatus(bool set)
    {
        isInDialogue = set;

        if(!NPC.ActiveNPC || NPC.ActiveNPC?.SpeakerData().SpeakerID() != SpeakerID.Stellan){
            RunGameTimer(!set, !set);
        }
    }
}