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
    
    private float smoothingVelocity;
    private float horizontalMove;
    private float verticalMove;

    public Animator animator;

    private bool movingLeft, movingRight, movingUp, movingDown = false;
    
    void Start()
    {
        InputActionAsset controls = gameObject.GetComponent<PlayerInput>().actions;

        // Add STOPPING moving when you're no longer holding the button
        controls.FindAction("MoveLeft").canceled += x => OnMoveLeftCanceled();
        controls.FindAction("MoveRight").canceled += x => OnMoveRightCanceled();
        controls.FindAction("MoveUp").canceled += x => OnMoveUpCanceled();
        controls.FindAction("MoveDown").canceled += x => OnMoveDownCanceled();
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
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(model.eulerAngles.y, targetAngle, ref smoothingVelocity, smoothing);
            model.rotation = Quaternion.Euler(0, angle, 0);
            player.Move(direction * speed * Time.deltaTime);
        }
    }
}
