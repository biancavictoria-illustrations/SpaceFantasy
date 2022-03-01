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

    private EnemyHealthBar enemyHealthUI;
    public bool isBossEnemy {get; private set;}
    public EnemyID enemyID {get; private set;}

    void Start()
    {
        OnDeath.AddListener(onEntityDeath);
        isBossEnemy = false;
        enemyID = EnemyID.enumSize;
        
        if(gameObject.tag != "Player"){
            enemyID = GetComponent<EnemyStats>().enemyID;
            // If a boss enemy (update int if we add more bosses)
            if( (int)enemyID < 3 ){
                isBossEnemy = true;
            }
            // If normal enemy
            else{
                enemyHealthUI = gameObject.GetComponentInChildren<EnemyHealthBar>();
                if(enemyHealthUI == null){
                    Debug.LogError("No enemy health UI found for normal enemy unit!");
                    return;
                }
            }
            SetMaxHealthUI();
            SetCurrentHealthUI();
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
        else if(!isBossEnemy){
            enemyHealthUI.SetCurrentHealth(currentHitpoints);
        }
        else{
            InGameUIManager.instance.bossHealthBar.SetCurrentHealth(currentHitpoints);
        }
    }

    public void SetMaxHealthUI()
    {
        if(gameObject.tag == "Player"){
            InGameUIManager.instance.SetMaxHealthValue(maxHitpoints);
        }
        else if(!isBossEnemy){
            enemyHealthUI.SetMaxHealth(maxHitpoints);
        }
        else{
            InGameUIManager.instance.bossHealthBar.SetMaxHealth(maxHitpoints);
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
            if(isBossEnemy){
                InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(false);
            }

            if(enemyDropGenerator){
                enemyDropGenerator.GetDrop(GameManager.instance.bossesKilled, transform);
            }
            else{
                Debug.LogWarning("No enemy drop generator found for enemy: " + enemyID);
            }
            
            Destroy(gameObject);
        }
        
    }
}
