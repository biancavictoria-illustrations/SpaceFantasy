using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public float movementSpeed = 1; // To Be set by enemy stats later
    public float provokedRange = 7;
    public float attackRange = 4;
    public float damage = 0.5f;

    private float windUp = 0.75f;
    private float duration = 0.25f;
    private float windDown = 0.5f;
    private float coolDown = 0.25f;
    private bool coroutineRunning = false;

    private GroundPathing path;
    private EntityAttack baseAttack;

    [SerializeField] private GameObject timerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        path = gameObject.GetComponent<GroundPathing>();
        path.speed = movementSpeed;
        path.provokedRadius = provokedRange;
        path.attackRadius = attackRange;

        baseAttack = gameObject.GetComponent<EntityAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        if(path.Provoked() && !coroutineRunning) // Update for damage later
        {
            Debug.Log("in here");
            coroutineRunning = true;
            StartCoroutine(SlimeLogic());
            StartCoroutine(CallDamage());
        }
    }

    private IEnumerator SlimeLogic()
    {
        //Debug.Log("chasing");
        yield return new WaitUntil(() => path.InAttackRange() && !path.attacking);
        //Debug.Log("Attacking");
        path.attacking = true;
        StartCoroutine(baseAttack.Attack(Instantiate(timerPrefab).GetComponent<Timer>(), false, windUp, duration, windDown, coolDown));
        yield return new WaitUntil(() => baseAttack.Completed);
        path.attacking = false;
        coroutineRunning = false;
    }

    private IEnumerator CallDamage()
    {
        yield return new WaitUntil(() => baseAttack.hit);
        baseAttack.enemyDeath = DealDamage();
        baseAttack.damageDealt = true;
    }

    private bool DealDamage()
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit, attackRange))
        {
            Debug.Log("hit");
            return hit.collider.tag == "Player" ? hit.collider.GetComponent<EntityHealth>().Damage(damage) : false;
        }
        else
        {
            Debug.Log("miss");
            return false;
        }
    }
}
