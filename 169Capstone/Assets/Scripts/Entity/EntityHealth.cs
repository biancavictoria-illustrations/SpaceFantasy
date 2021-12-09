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

    public float maxHitpoints;
    public float currentHitpoints;

    [SerializeField] private Drop drop;
    //[SerializeField] private ObjectManager objectManager;

    private EnemyHealthBar enemyHealthUI;

    void Start()
    {
        Debug.Log(maxHitpoints);
        OnDeath.AddListener(onEntityDeath);
        
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
    }

    public bool Damage(float damage)
    {
        currentHitpoints -= damage;
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
            //Debug.Log(drop.GetDrop(ObjectManager.bossesKilled));
        }

        Destroy(gameObject);
    }
}
