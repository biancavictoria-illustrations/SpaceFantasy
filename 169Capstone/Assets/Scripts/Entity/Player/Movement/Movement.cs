using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5;
    public Transform model;
    public CharacterController player;
    public float smoothing = 0.1f;

    private float inputVertical;
    private float inputHorizontal;
    private float smoothingVelocity;

    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        inputVertical = Input.GetAxisRaw("Horizontal");
        inputHorizontal = Input.GetAxisRaw("Vertical");

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
        Vector3 direction = new Vector3(-inputHorizontal, 0, inputVertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float rad = 45 * Mathf.Deg2Rad;
            if(inputHorizontal > 0)
            {
                direction.z -= rad;
            }
            else if(inputHorizontal < 0)
            {
                direction.z += rad;
            }

            if(inputVertical > 0)
            {
                direction.x -= rad;
            }
            else if(inputVertical < 0)
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
