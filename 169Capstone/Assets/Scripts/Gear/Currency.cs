using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public enum CurrencyType
    {
        Coin,
        StarShard
    }

    [SerializeField] private float initialForce = 1;
    [SerializeField] private bool gravitateToPlayer;
    [SerializeField] private bool isGravitating;
    [SerializeField] private float maxGravitationRange;
    [SerializeField] private Vector2 gravitateDelayRange = new Vector2(0.5f, 1f);
    [SerializeField] private CurrencyType type;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Quaternion.Euler(Random.Range(-20, 20), 0, Random.Range(-20, 20)) * Vector3.up * initialForce;
        rb.angularVelocity = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
    }

    void FixedUpdate()
    {
        if(gravitateToPlayer && (maxGravitationRange <= 0 || Vector3.Distance(Player.instance.transform.position, transform.position) < maxGravitationRange) && !isGravitating)
        {
            gameObject.layer = LayerMask.NameToLayer("Currency");
            isGravitating = true;
            StartCoroutine(gravitateTowardPlayerRoutine());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            switch (type)
            {
                case CurrencyType.Coin:
                    PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.tempCurrency + 1);
                    break;
                case CurrencyType.StarShard:
                    PlayerInventory.instance.SetPermanentCurrency(PlayerInventory.instance.permanentCurrency + 1);
                    break;
            }

            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            switch (type)
            {
                case CurrencyType.Coin:
                    PlayerInventory.instance.SetTempCurrency(PlayerInventory.instance.tempCurrency + 1);
                    break;
                case CurrencyType.StarShard:
                    PlayerInventory.instance.SetPermanentCurrency(PlayerInventory.instance.permanentCurrency + 1);
                    break;
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator gravitateTowardPlayerRoutine()
    {
        yield return new WaitForSeconds(Random.Range(gravitateDelayRange.x, gravitateDelayRange.y));

        float lookProgress = 0;
        Quaternion original = Quaternion.LookRotation(rb.velocity);
        GetComponent<Collider>().isTrigger = true;
        while(true)
        {
            yield return new WaitForFixedUpdate();
            if(lookProgress < 1)
                lookProgress += 0.05f;
            rb.velocity = Quaternion.Lerp(original, Quaternion.LookRotation(Player.instance.transform.position + Vector3.up - transform.position), lookProgress) * Vector3.forward * (rb.velocity.magnitude + 1f);
        }
    }
}
