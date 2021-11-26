using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public float maxHitpoints;
    public float currentHitpoints;
    private bool startCoroutine = true;
    [SerializeField] private Drop drop;

    [SerializeField] private ObjectManager objectManager;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(maxHitpoints);
        //StartCoroutine(Death());
    }

    // Update is called once per frame
    void Update()
    {
        if (startCoroutine && currentHitpoints == maxHitpoints)
        {
            startCoroutine = false;
            StartCoroutine(Death());
        }
    }

    public bool Damage(float damage)
    {
        currentHitpoints -= damage;
        Debug.Log("Hitpoints");
        Debug.Log(currentHitpoints);
        return currentHitpoints <= 0;
    }

    public void Heal(float health)
    {
        currentHitpoints += health;

        if(currentHitpoints > maxHitpoints)
        {
            currentHitpoints = maxHitpoints;
        }
    }

    private IEnumerator Death()
    {
        yield return new WaitUntil(() => currentHitpoints <= 0 && maxHitpoints > 0);
        Debug.Log("Death");
        if(gameObject.tag == "Player")
        {
            objectManager.playerDeath = true;
        }
        else
        {
            Debug.Log(drop.GetDrop(ObjectManager.bossesKilled));
        }

        Destroy(gameObject);
    }
}
