using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Animator animator;
    public float speed = 5;
    public Transform model;
    public CharacterController player;
    public float smoothing = 0.1f;
    public float gravityAccel = -10f;
    public float jumpSpeed = 10;
    
    private float smoothingVelocity;
    private float horizontalMove;
    private float verticalMove;
    private float fallingVelocity;
    private bool isGrounded;
    private bool isJumping;

    private bool movingLeft, movingRight, movingUp, movingDown = false;
    
    void Start()
    {
        InputActionAsset controls = GetComponent<PlayerInput>().actions;

        // Add STOPPING moving when you're no longer holding the button
        controls.FindAction("MoveLeft").canceled += x => OnMoveLeftCanceled();
        controls.FindAction("MoveRight").canceled += x => OnMoveRightCanceled();
        controls.FindAction("MoveUp").canceled += x => OnMoveUpCanceled();
        controls.FindAction("MoveDown").canceled += x => OnMoveDownCanceled();
        controls.FindAction("Jump").canceled += x => OnJumpCanceled();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f, LayerMask.GetMask("RoomBounds"));

        foreach(Collider col in colliders)
        {
            if(col.bounds.Contains(transform.position))
            {
                Room roomScript = col.GetComponent<Room>();
                if(roomScript != null)
                    AudioManager.Instance.playMusic(AudioManager.MusicTrack.Level1, roomScript.hasEnemies());
                break;
            }
        }
    }
    
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f, LayerMask.GetMask("RoomBounds"));

        foreach(Collider col in colliders)
        {
            if(col.bounds.Contains(transform.position))
            {
                Room roomScript = col.GetComponent<Room>();
                if(roomScript != null)
                    AudioManager.Instance.toggleCombat(roomScript.hasEnemies());
                break;
            }
        }
    }

    void FixedUpdate()
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        HandleMovement();
    }

    public void OnMoveLeft(InputValue input)
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        movingLeft = true;
        if(input != null){
            horizontalMove = input.Get<float>();
        }
        else{
            horizontalMove = -1f;            
        }
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveLeftCanceled()
    {
        movingLeft = false;
        if(!movingRight){
            horizontalMove = 0;
            CheckForIdle();
        }
        else{
            OnMoveRight(null);
        }
    }

    public void OnMoveRight(InputValue input)
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        movingRight = true;
        if(input != null){
            horizontalMove = input.Get<float>();
        }
        else{            
            horizontalMove = 1f;
        }
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveRightCanceled()
    {
        movingRight = false;
        if(!movingLeft){
            horizontalMove = 0;
            CheckForIdle();
        }
        else{
            OnMoveLeft(null);
        }
    }

    public void OnMoveUp(InputValue input)
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        movingUp = true;
        if(input != null){
            verticalMove = input.Get<float>();            
        }
        else{
            verticalMove = 1f;
        }
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveUpCanceled()
    {
        movingUp = false;
        if(!movingDown){
            verticalMove = 0;
            CheckForIdle();
        }
        else{
            OnMoveDown(null);
        }
    }

    public void OnMoveDown(InputValue input)
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        movingDown = true;
        if(input != null){
            verticalMove = input.Get<float>();            
        }
        else{
            verticalMove = -1f;
        }
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveDownCanceled()
    {
        movingDown = false;
        if(!movingUp){
            verticalMove = 0;
            CheckForIdle();
        }
        else{
            OnMoveUp(null);
        }
    }

    public void OnJump(InputValue input)
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }

        if(isGrounded)
        {
            fallingVelocity = jumpSpeed * Time.fixedDeltaTime;
            isGrounded = false;
            isJumping = true;
        }
    }

    public void OnJumpCanceled()
    {
        //If we need to put code for what happens when the player lets go of jump
    }

    private void CheckForIdle()
    {
        if(verticalMove == 0 && horizontalMove == 0){
            animator.SetBool("IsRunning", false);
        }
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(-verticalMove, 0, horizontalMove).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float rad = 45 * Mathf.Deg2Rad;
            if(verticalMove > 0)
            {
                direction.z -= rad;
            }
            else if(verticalMove < 0)
            {
                direction.z += rad;
            }

            if(horizontalMove > 0)
            {
                direction.x -= rad;
            }
            else if(horizontalMove < 0)
            {
                direction.x += rad;
            }

            direction *= speed * Time.fixedDeltaTime;
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(model.eulerAngles.y, targetAngle, ref smoothingVelocity, smoothing);

            if(InputManager.instance.isAttacking)
            {
                model.rotation = Quaternion.FromToRotation(Vector3.forward, InputManager.instance.cursorLookDirection);
                direction /= 2;
            }
            else
            {
                model.rotation = Quaternion.Euler(0, angle, 0);
            }
        }
        else
        {
            direction = Vector3.zero;
        }

        if(!isJumping && Physics.Raycast(transform.position, Vector3.down, 0.1f, LayerMask.GetMask("Environment")))
        {
            fallingVelocity = -10000;
            isGrounded = true;
        }
        else
        {
            if(isGrounded)
            {
                fallingVelocity = 0;
                isGrounded = false;
            }

            fallingVelocity += gravityAccel * Time.fixedDeltaTime;
        }

        if(fallingVelocity < 0)
        {
            isJumping = false;
        }

        direction += Vector3.up * fallingVelocity;
        player.Move(direction);
    }
}
