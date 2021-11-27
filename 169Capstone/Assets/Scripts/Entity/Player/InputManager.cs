using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // This stuff can either be here OR just in the Movement script and this script tells that one to handle input based on the input
    // it receives (like OnMoveLeft could call something like Movement.HandleMovement(inputValue); that does the calculations and the movement)
    public float speed = 5;
    public Transform model;
    public CharacterController player;
    public float smoothing = 0.1f;
    private float smoothingVelocity;

    public Animator animator;

    public SpeakerData speakerData;     // This should be stored in a different player script

    private bool isInDialogue;
    private bool inventoryIsOpen;
    private bool shopIsOpen;
    private bool compareItemIsOpen;

    public PauseMenu pauseMenu;     // Should that be a singleton instead?

    void Start()
    {
        DialogueManager.instance.AddSpeaker(speakerData);
        InputActionAsset controls = gameObject.GetComponent<PlayerInput>().actions;
        inventoryIsOpen = false;
        isInDialogue = false;

        // Add STOPPING moving when you're no longer holding the button
        controls.FindAction("MoveLeft").canceled += x => OnMoveLeftCanceled();
        controls.FindAction("MoveRight").canceled += x => OnMoveRightCanceled();
        controls.FindAction("MoveUp").canceled += x => OnMoveUpCanceled();
        controls.FindAction("MoveDown").canceled += x => OnMoveDownCanceled();
        
        controls.FindAction("Jump").canceled += x => OnJumpCanceled();
        controls.FindAction("Attack").canceled += x => OnAttackCanceled();
    }

    private bool CanAcceptGameplayInput()
    {
        if(isInDialogue || PauseMenu.GameIsPaused || inventoryIsOpen || shopIsOpen || compareItemIsOpen){
            return false;
        }
        return true;
    }

    // This could be in a separate script...
    public void HandleMovement(float vertical, float horizontal)
    {
        Vector3 direction = new Vector3(-horizontal, 0, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float rad = 45 * Mathf.Deg2Rad;
            if(horizontal > 0)
            {
                direction.z -= rad;
            }
            else if(horizontal < 0)
            {
                direction.z += rad;
            }

            if(vertical > 0)
            {
                direction.x -= rad;
            }
            else if(vertical < 0)
            {
                direction.x += rad;
            }
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(model.eulerAngles.y, targetAngle, ref smoothingVelocity, smoothing);
            model.rotation = Quaternion.Euler(0, angle, 0);
            player.Move(direction * speed * Time.deltaTime);
        }
    }

    public void OnMoveLeft(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // TODO: Move
        HandleMovement(0,input.Get<float>());

        animator.SetBool("IsRunning", true);
    }

    public void OnMoveLeftCanceled()
    {
        // TODO: Stop moving
        HandleMovement(0,0);

        animator.SetBool("IsRunning", false);
    }

    public void OnMoveRight(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // TODO: Move
        HandleMovement(0,input.Get<float>());

        animator.SetBool("IsRunning", true);
    }

    public void OnMoveRightCanceled()
    {
        // TODO: Stop moving
        HandleMovement(0,0);

        animator.SetBool("IsRunning", false);
    }

    public void OnMoveUp(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // TODO: Move
        HandleMovement(input.Get<float>(),0);

        animator.SetBool("IsRunning", true);
    }

    public void OnMoveUpCanceled()
    {
        // TODO: Stop moving
        HandleMovement(0,0);

        animator.SetBool("IsRunning", false);
    }

    public void OnMoveDown(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        // TODO: Move
        HandleMovement(input.Get<float>(),0);

        animator.SetBool("IsRunning", true);
    }

    public void OnMoveDownCanceled()
    {
        // TODO: Stop moving
        HandleMovement(0,0);

        animator.SetBool("IsRunning", false);
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

        // TODO: Attack

        // animator.SetBool("IsAttacking", true);
        // Debug.Log(animator.GetLayerIndex("Slashing"));
        // gameObject.GetComponent<Animator>().Rebind();
        // animator.SetLayerWeight(1, 1);
    }

    public void OnAttackCanceled()
    {
        // TODO: Stop attacking

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

    public void OnPause(InputValue input)
    {
        if(PauseMenu.GameIsPaused){
            pauseMenu.ResumeGame();
        }
        else{
            pauseMenu.PauseGame();
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
        isInDialogue = true;    // Used to pause player movement/interactions while in dialogue
        DialogueManager.instance.dialogueRunner.StartDialogue(NPC.ActiveNPC.yarnStartNode);
    }

    // Called automatically when the dialogue ends/closes
    public void EndDialogue()
    {
        isInDialogue = false;   // Unpause player control
        NPC.ActiveNPC.TalkedToNPC();
    }
}
