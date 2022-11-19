using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayer : MonoBehaviour
{

    public float damage = 5f;
    public float turnRate = 10.0f;//90.0f;
    public float turnIntervalRate = 5.0f;
    public Transform playerTransform;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public Transform projectileSpawnPoint2;
    public Transform projectileSpawnPoint3;
    public Transform projectileSpawnPoint4;
    public float timer;// = 5;
    private float timeCount = 0;
    private Vector3 from;
    private Vector3 to;
    public bool turn = true;
    public bool shootPlayer = false;
    public float delayRate = 2.0f;
    public float shootRate = 3.0f;

    [SerializeField][FMODUnity.EventRef] private string shootSFX;

    void Awake()
    {
        // If main level (do this in awake to add a listener)
        if(GameManager.instance.InSceneWithRandomGeneration()){
            FloorGenerator generator = FindObjectOfType<FloorGenerator>();
            if(generator){  // If we already completed generation, just run start; if not, add a listener
                if(generator.generationComplete){
                    StartOnGenerationComplete();
                }
                else{
                    generator.OnGenerationComplete.AddListener(StartOnGenerationComplete);
                }
            }
        }
    }

    void Start()
    {
        // If lich fight arena or something
        if(!GameManager.instance.InSceneWithRandomGeneration()){
            StartOnGenerationComplete();
        }
    }

    public void StartOnGenerationComplete()
    {
        playerTransform = Player.instance.transform;
        from = transform.localRotation.eulerAngles;
        to = from;
        to.y += 90;
    }

    // Start is called before the first frame update
    private void Update()
    {
        //timer -=Time.deltaTime;
        if (turn && !shootPlayer)
        {
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(from), Quaternion.Euler(to), timeCount * turnRate);
            timeCount += Time.deltaTime;

           
        }

         if(transform.localRotation.eulerAngles == to && turn && !shootPlayer)
            {
                turn = false;
                from = to;
                to.y += 90;

                if (to.y == 360)
                    to.y = 0;

                timeCount = 0;

                StartCoroutine(waitInterval());
            }

        if(shootPlayer && timer <= 0)
        {
            timer = shootRate;
            ShootProjectile();
        }
        else if(shootPlayer)
        {
            timer -= Time.deltaTime;
        }
        
    }

    /*private void FixedUpdate() //need to adjujst this
    {
        Vector3 localTargetPos = transform.InverseTransformPoint(playerTransform.position);
        localTargetPos.y = 0.0f;
        Quaternion rotationGoal = Quaternion.LookRotation(localTargetPos);
        
        Quaternion newRotation = Quaternion.RotateTowards(transform.localRotation, rotationGoal, turnRate * Time.deltaTime);

        // Set the new rotation of the base.
        transform.localRotation = newRotation;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        // If player transform is null at this point, set it
        if(!playerTransform){
            playerTransform = Player.instance.transform;
        }

        /*timer = 5;
        if (other.tag == "Player")
        {
            ShootProjectile();
        }*/
    }

    /*private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            if(timer <= 0)
            {
                ShootProjectile();
                timer = 5;
            }
            
        }

    }*/

    public void ShootProjectile()
    {
        AudioManager.Instance.PlaySFX(shootSFX, gameObject);

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

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), new DamageData(damage, false), DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
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

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), new DamageData(damage, false), DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
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

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), new DamageData(damage, false), DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
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

            projectileScript.Initialize(LayerMask.NameToLayer("Player"), new DamageData(damage, false), DamageSourceType.TurretTrap, playerTransform.position + Vector3.up * 2 - projectileSpawnPoint.position);
        }
        
        
        
    }

    private IEnumerator waitInterval()
    {
        yield return new WaitForSeconds(turnIntervalRate);

        turn = true;
    }
}
