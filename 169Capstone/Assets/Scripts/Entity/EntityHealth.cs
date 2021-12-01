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

    private EnemyHealthBar enemyHealthUI;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(maxHitpoints);
        //StartCoroutine(Death());

        if(gameObject.tag != "Player"){
            enemyHealthUI = gameObject.GetComponentInChildren<EnemyHealthBar>();
            if(enemyHealthUI == null){
                Debug.LogError("No enemy health UI found for unit!");
                return;
            }
        }

        SetMaxHealthUI();
        SetCurrentHealthUI();
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
        
        SetCurrentHealthUI();

        return currentHitpoints <= 0;
    }

    public void Heal(float health)
    {
        currentHitpoints += health;

        if(currentHitpoints > maxHitpoints)
        {
            currentHitpoints = maxHitpoints;
        }

        SetCurrentHealthUI();
    }

    public void SetCurrentHealthUI()
    {
        if(gameObject.tag == "Player"){
            InGameUIManager.instance.SetCurrentHealthValue(currentHitpoints);   
        }
        else{
            enemyHealthUI.SetCurrentHealth(currentHitpoints);
        }
    }

    public void SetMaxHealthUI()
    {
        if(gameObject.tag == "Player"){
            InGameUIManager.instance.SetMaxHealthValue(maxHitpoints);
        }
        else{
            enemyHealthUI.SetMaxHealth(maxHitpoints);
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
