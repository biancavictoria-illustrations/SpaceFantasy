using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5;
    public Transform model;
    public CharacterController player;
    public float smoothing = 0.1f;

    private float smoothingVelocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("Horizontal");
        float horizontal = Input.GetAxisRaw("Vertical");
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
}
