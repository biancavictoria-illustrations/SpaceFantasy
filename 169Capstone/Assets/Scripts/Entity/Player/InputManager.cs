using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        InputActionAsset controls = gameObject.GetComponent<PlayerInput>().actions;

        // Add STOPPING when you're no longer holding the button
        controls.FindAction("AttackPrimary").canceled += x => OnAttackPrimaryCanceled();
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
        else if(NPC.ActiveNPC && !NPC.ActiveNPC.HaveTalkedToNPC()){
            DialogueManager.instance.OnNPCInteracted();
        }

        // If you're in range of a door, walk through it
        else if(SceneTransitionDoor.ActiveDoor){
            SceneTransitionDoor.ActiveDoor.ChangeScene();
        }
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
    }

    public void OnUseHealthPotion(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // TODO: Use health potion
    }

    public void OnUseOtherPotion(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // TODO: Use your other potion, if you have one
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
}
