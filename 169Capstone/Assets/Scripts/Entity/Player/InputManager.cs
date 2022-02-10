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
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool useAccessory = false;
    [HideInInspector] public bool useHead = false;
    [HideInInspector] public bool useLegs = false;

    public bool latestInputIsController {get; private set;}

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
    }

    void Start()
    {
        InputActionAsset controls = GetComponent<PlayerInput>().actions;

        player = FindObjectOfType<Player>();

        // Add STOPPING when you're no longer holding the button
        controls.FindAction("AttackPrimary").canceled += x => OnAttackPrimaryCanceled();
    }

    void Update()
    {
        if(!isAttacking)
        {
            Vector2 playerPositionOnScreen = (Vector2)Camera.main.WorldToScreenPoint(player.transform.position); //Get Player's position on the screen
            Vector2 cursorLookDirection2D = (mousePos - playerPositionOnScreen).normalized;                      //Get the vector from the player position to the cursor position 
            Vector3 lookDirectionRelativeToCamera = Camera.main.transform.right * cursorLookDirection2D.x + Camera.main.transform.up * cursorLookDirection2D.y; //Create the look vector in 3D space relative to the camera
            cursorLookDirection = Quaternion.FromToRotation(-Camera.main.transform.forward, Vector3.up) * lookDirectionRelativeToCamera;                        //Rotate the look vector to be in terms of world space
        }
    }

    public void UpdateLatestInputDevice()
    {
        if((int)UserDeviceManager.currentControlDevice == 0){
            latestInputIsController = false;
            Debug.Log("input device is now keyboard/mouse");
        }
        else{
            latestInputIsController = true;
            Debug.Log("input device is now a controller");
        }
    }

    public bool CanAcceptGameplayInput()
    {
        if(isInDialogue || PauseMenu.GameIsPaused || inventoryIsOpen || shopIsOpen || compareItemIsOpen){
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
            InGameUIManager.instance.OpenNPCShop(NPC.ActiveNPC.SpeakerData());
        }

        // If you're in range of a door, walk through it
        else if(SceneTransitionDoor.ActiveDoor){
            SceneTransitionDoor.ActiveDoor.ChangeScene();
        }

        // Check if you're in range of a dropped item
        else if(GeneratedEquipment.ActiveGearDrop){
            GeneratedEquipment item = GeneratedEquipment.ActiveGearDrop;

            // If the player doesn't have anything equipped in that slot, equip it
            if( !PlayerInventory.instance.gear[item.equipmentData.ItemSlot()] ){
                PlayerInventory.instance.EquipItem(item.equipmentData.ItemSlot(), item);
            }
            else{   // Otherwise, open compare UI
                ToggleCompareItemUI(true, item);
            }
        }
    }

    public void ToggleCompareItemUI(bool set, GeneratedEquipment item)
    {
        compareItemIsOpen = set;
        InGameUIManager.instance.SetGearSwapUIActive(set, item);
    }

    // If you're in dialogue, click anywhere to progress
    public void OnClick(InputValue input)
    {
        if(isInDialogue && !PauseMenu.GameIsPaused){
            DialogueManager.instance.dialogueUI.MarkLineComplete();
        }
    }

    public void OnPause(InputValue input)
    {
        if(PauseMenu.GameIsPaused){
            InGameUIManager.instance.pauseMenu.ResumeGame();
        }
        else{
            InGameUIManager.instance.pauseMenu.PauseGame();
        }
    }

    public void OnToggleInventory(InputValue input)
    {
        if(isInDialogue || PauseMenu.GameIsPaused || shopIsOpen || compareItemIsOpen){
            return;
        }
        
        InGameUIManager.instance.SetInventoryUIActive(!InGameUIManager.instance.inventoryIsOpen);
        inventoryIsOpen = !inventoryIsOpen;

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
        if(!CanAcceptGameplayInput())
        {
            return;
        }

        useAccessory = true;
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
}
