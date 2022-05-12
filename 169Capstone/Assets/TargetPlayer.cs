using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayer : MonoBehaviour
{

    public float damage = 5f;
    public float turnRate = 90.0f;
    public Transform playerTransform;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public Transform projectileSpawnPoint2;
    public Transform projectileSpawnPoint3;
    public Transform projectileSpawnPoint4;
    private float timer = 5;

    void Start()
    {
        playerTransform = Player.instance.transform;
    }

    // Start is called before the first frame update
    private void Update()
    {
        timer -=Time.deltaTime;
        
    }

    private void FixedUpdate()
    {
        Vector3 localTargetPos = transform.InverseTransformPoint(playerTransform.position);
        localTargetPos.y = 0.0f;
        Quaternion rotationGoal = Quaternion.LookRotation(localTargetPos);
        
        Quaternion newRotation = Quaternion.RotateTowards(transform.localRotation, rotationGoal, turnRate * Time.deltaTime);

        // Set the new rotation of the base.
        transform.localRotation = newRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If player transform is null at this point, set it
        if(!playerTransform){
            playerTransform = Player.instance.transform;
        }

        timer = 5;
        if (other.tag == "Player")
        {
            ShootProjectile();
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            if(timer <= 0)
            {
                ShootProjectile();
                timer = 5;
            }
            
        }

    }

    public void ShootProjectile()
    {
        int proSpawn = Random.Range(1, 5);
        if (proSpawn == 1)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.FromToRotation(transform.position, playerTransform.position));
            Projectile projectileScript = projectileObject.GetComponent<Projectile>();
            if (!projectileScript)
            {
                Destroy(projectileObject);
                Debug.LogError("Projectile prefab " + projectilePrefab + " did not contain a Projectile script.");
            }

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), damage, DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
        }
        else if (proSpawn == 2)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint2.position, Quaternion.FromToRotation(transform.position, playerTransform.position));
            Projectile projectileScript = projectileObject.GetComponent<Projectile>();
            if (!projectileScript)
            {
                Destroy(projectileObject);
                Debug.LogError("Projectile prefab " + projectilePrefab + " did not contain a Projectile script.");
            }

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), damage, DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
        }
        else if (proSpawn == 3)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint3.position, Quaternion.FromToRotation(transform.position, playerTransform.position));
            Projectile projectileScript = projectileObject.GetComponent<Projectile>();
            if (!projectileScript)
            {
                Destroy(projectileObject);
                Debug.LogError("Projectile prefab " + projectilePrefab + " did not contain a Projectile script.");
            }

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), damage, DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
        }
        else if (proSpawn == 4)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint4.position, Quaternion.FromToRotation(transform.position, playerTransform.position));
            Projectile projectileScript = projectileObject.GetComponent<Projectile>();
            if (!projectileScript)
            {
                Destroy(projectileObject);
                Debug.LogError("Projectile prefab " + projectilePrefab + " did not contain a Projectile script.");
            }

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), damage, DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
        }
        
        
        
    }
}
