using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public SpeakerData speakerData;     // This should be stored in a different player script

    [HideInInspector] public bool isInDialogue = false;
    [HideInInspector] public bool inventoryIsOpen = false;
    [HideInInspector] public bool shopIsOpen = false;
    [HideInInspector] public bool compareItemIsOpen = false;
    [HideInInspector] public bool isAttacking = false;

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
        DialogueManager.instance.AddSpeaker(speakerData);
        InputActionAsset controls = gameObject.GetComponent<PlayerInput>().actions;

        // Add STOPPING moving when you're no longer holding the button
        controls.FindAction("Jump").canceled += x => OnJumpCanceled();
        controls.FindAction("Attack").canceled += x => OnAttackCanceled();
    }

    public bool CanAcceptGameplayInput()
    {
        if(isInDialogue || PauseMenu.GameIsPaused || inventoryIsOpen || shopIsOpen || compareItemIsOpen){
            return false;
        }
        return true;
    }

    public void OnJump(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // TODO: Jump
    }

    public void OnJumpCanceled()
    {
        // TODO: Stop jumping
    }

    public void OnAttack(InputValue input)
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

    public void OnAttackCanceled()
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
            StartDialogue();
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

    // Called when the player clicks the interact button in range of an NPC with something to say
    private void StartDialogue()
    {
        isInDialogue = true;
        DialogueManager.instance.dialogueRunner.StartDialogue(NPC.ActiveNPC.yarnStartNode);
    }

    // Called automatically when the dialogue ends/closes
    public void OnDialogueEnd()
    {
        isInDialogue = false;
        NPC.ActiveNPC.TalkedToNPC();
    }
}
