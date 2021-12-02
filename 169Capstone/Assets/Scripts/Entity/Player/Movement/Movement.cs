using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed = 5;
    public Transform model;
    public CharacterController player;
    public float smoothing = 0.1f;
    private float smoothingVelocity;
    
    private float horizontalMove;
    private float verticalMove;

    public Animator animator;
    
    void Start()
    {
        InputActionAsset controls = gameObject.GetComponent<PlayerInput>().actions;

        // Add STOPPING moving when you're no longer holding the button
        controls.FindAction("MoveHorizontal").canceled += x => OnMoveHorizontalCanceled();
        controls.FindAction("MoveVertical").canceled += x => OnMoveVerticalCanceled();
    }

    void Update()
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        HandleMovement();
    }

    public void OnMoveHorizontal(InputValue input)
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        verticalMove = input.Get<float>();
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveHorizontalCanceled()
    {
        verticalMove = 0;
        CheckForIdle();
    }

    public void OnMoveVertical(InputValue input)
    {
        if(!InputManager.instance.CanAcceptGameplayInput()){
            return;
        }
        horizontalMove = input.Get<float>();
        animator.SetBool("IsRunning", true);
    }

    public void OnMoveVerticalCanceled()
    {
        horizontalMove = 0;
        CheckForIdle();
    }

    private void CheckForIdle()
    {
        if(verticalMove == 0 && horizontalMove == 0){
            animator.SetBool("IsRunning", false);
        }
    }

    private void HandleMovement()
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
}
