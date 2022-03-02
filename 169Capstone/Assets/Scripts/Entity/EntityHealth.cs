using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    #region OnDeath
        public class OnDeathEvent : UnityEvent<EntityHealth> {}

        public OnDeathEvent OnDeath
        {
            get 
            { 
                if(onDeath == null) 
                    onDeath = new OnDeathEvent(); 
                
                return onDeath;
            }

            set { onDeath = value; }
        }
        private OnDeathEvent onDeath;
    #endregion

    #region OnHit
        public class OnHitEvent : UnityEvent<EntityHealth, float> {}

        public OnHitEvent OnHit
        {
            get 
            { 
                if(onHit == null) 
                    onHit = new OnHitEvent(); 
                
                return onHit;
            }

            set { onHit = value; }
        }
        private OnHitEvent onHit;
    #endregion

    public float maxHitpoints;
    public float currentHitpoints;

    [SerializeField] private EnemyDropGenerator enemyDropGenerator;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject starShardPrefab;

    private EnemyHealthBar enemyHealthUI;

    private const int TEMPCOINDROPAMOUNT = 5;
    private const float TEMPSTARSHARDDROPCHANCE = 0.25f;

    void Start()
    {
        OnDeath.AddListener(onEntityDeath);
        
        if(gameObject.tag != "Player"){
            enemyHealthUI = gameObject.GetComponentInChildren<EnemyHealthBar>();
            if(enemyHealthUI == null){
                Debug.LogError("No enemy health UI found for unit!");
                return;
            }
        }
    }

    public void SetStartingHealthUI()
    {
        if(gameObject.tag == "Player"){
            Debug.Log("Max Health At Start: " + maxHitpoints + "\nCurrent Health At Start: " + currentHitpoints);
        }

        SetMaxHealthUI();
        SetCurrentHealthUI();
    }

    public bool Damage(float damage)
    {
        currentHitpoints -= damage;
        OnHit.Invoke(this, damage);
        Debug.Log("Hitpoints");
        Debug.Log(currentHitpoints);
        
        SetCurrentHealthUI();

        if(currentHitpoints <= 0)
        {
            //Debug.Log("Dead");
            OnDeath.Invoke(this);
        }

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

    private void onEntityDeath(EntityHealth health)
    {
        if(health != this)
            return;

        Debug.Log("Death");
        if(gameObject.tag == "Player")
        {
            //objectManager.playerDeath = true;
            GameManager.instance.playerDeath = true;
        }
        else
        {
            enemyDropGenerator.GetDrop(GameManager.instance.bossesKilled, transform);

            for(int i = 0; i < TEMPCOINDROPAMOUNT; ++i)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }

            if(Random.value <= TEMPSTARSHARDDROPCHANCE)
                Instantiate(starShardPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
            //Debug.Log(drop.GetDrop(ObjectManager.bossesKilled));
        }
        
    }
}
