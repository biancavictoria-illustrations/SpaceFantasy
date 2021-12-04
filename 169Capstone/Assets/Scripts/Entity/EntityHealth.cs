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
    [SerializeField] private ObjectManager objectManager;
    
    void Start()
    {
        Debug.Log("onDeath event: " + OnDeath);
        Debug.Log(maxHitpoints);
        OnDeath.AddListener(onEntityDeath);
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

        if(currentHitpoints <= 0)
        {
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
    }

    private void onEntityDeath(EntityHealth health)
    {
        if(health != this)
            return;

        Debug.Log("Death");
        if(gameObject.tag == "Player")
        {
            objectManager.playerDeath = true;
        }
        else
        {
            //Debug.Log(drop.GetDrop(ObjectManager.bossesKilled));
        }

        Destroy(gameObject);
    }
}
