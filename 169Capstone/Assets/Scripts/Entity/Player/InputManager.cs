using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    // This stuff can either be here OR just in the Movement script and this script tells that one to handle input based on the input
    // it receives (like OnMoveLeft could call something like Movement.HandleMovement(inputValue); that does the calculations and the movement)
    public float speed = 5;
    public Transform model;
    public CharacterController player;
    public float smoothing = 0.1f;
    private float smoothingVelocity;
    private float horizontalMove;
    private float verticalMove;

    public Animator animator;

    public SpeakerData speakerData;     // This should be stored in a different player script

    [HideInInspector] public bool isInDialogue;
    [HideInInspector] bool inventoryIsOpen;
    [HideInInspector] bool shopIsOpen;
    [HideInInspector] bool compareItemIsOpen;

    public PauseMenu pauseMenu;     // Should that be a singleton instead?

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
        inventoryIsOpen = false;
        isInDialogue = false;

        // Add STOPPING moving when you're no longer holding the button
        controls.FindAction("MoveHorizontal").canceled += x => OnMoveHorizontalCanceled();
        controls.FindAction("MoveVertical").canceled += x => OnMoveVerticalCanceled();
        controls.FindAction("Jump").canceled += x => OnJumpCanceled();
        controls.FindAction("Attack").canceled += x => OnAttackCanceled();
    }

    void Update()
    {
        if(!CanAcceptGameplayInput()){
            return;
        }
        HandleMovement();
    }

    private bool CanAcceptGameplayInput()
    {
        if(isInDialogue || PauseMenu.GameIsPaused || inventoryIsOpen || shopIsOpen || compareItemIsOpen){
            return false;
        }
        return true;
    }

    // This could be in a separate script...
    public void HandleMovement()
    {
        Vector3 direction = new Vector3(-horizontalMove, 0, verticalMove).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float rad = 45 * Mathf.Deg2Rad;
            if(horizontalMove > 0)
            {
                direction.z -= rad;
            }
            else if(horizontalMove < 0)
            {
                direction.z += rad;
            }

            if(verticalMove > 0)
            {
                direction.x -= rad;
            }
            else if(verticalMove < 0)
            {
                direction.x += rad;
            }
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(model.eulerAngles.y, targetAngle, ref smoothingVelocity, smoothing);
            model.rotation = Quaternion.Euler(0, angle, 0);
            player.Move(direction * speed * Time.deltaTime);
        }
    }

    public void OnMoveHorizontal(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        verticalMove = input.Get<float>();
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveHorizontalCanceled()
    {
        verticalMove = 0;
        if(horizontalMove == 0){
            animator.SetBool("IsRunning", false);
        }
    }

    public void OnMoveVertical(InputValue input)
    {
        if(!CanAcceptGameplayInput()){
            return;
        }

        horizontalMove = input.Get<float>();
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveVerticalCanceled()
    {
        horizontalMove = 0;
        if(verticalMove == 0){
            animator.SetBool("IsRunning", false);
        }
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
        isInDialogue = true;
        DialogueManager.instance.dialogueRunner.StartDialogue(NPC.ActiveNPC.yarnStartNode);
    }

    // Called automatically when the dialogue ends/closes
    public void EndDialogue()
    {
        isInDialogue = false;
        NPC.ActiveNPC.TalkedToNPC();
    }
}
