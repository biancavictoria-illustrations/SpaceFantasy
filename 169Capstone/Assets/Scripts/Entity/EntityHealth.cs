﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    #region OnCrit
        public class OnCritEvent : UnityEvent<EntityHealth, float> {}

        public OnCritEvent OnCrit
        {
            get
            {
                if (onCrit == null)
                    onCrit = new OnCritEvent();

                return onCrit;
            }

            set { onCrit = value; }
        }
        private OnCritEvent onCrit;
    #endregion

    public float maxHitpoints;
    public float currentHitpoints;

    [SerializeField] private EnemyDropGenerator enemyDropGenerator;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject starShardPrefab;

    private EnemyHealthBar enemyHealthUI;
    public bool isBossEnemy {get; private set;}
    public EnemyID enemyID {get; private set;}

    private const int TEMPCOINDROPAMOUNT = 5;
    private const float TEMPSTARSHARDDROPCHANCE = 0.25f;

    public bool tempPlayerGodModeToggle = false;    // FOR TESTING - REMOVE THIS FOR FINAL BUILD

    void Start()
    {
        OnDeath.AddListener(onEntityDeath);
        OnCrit.AddListener(onPlayerCrit);
        isBossEnemy = false;
        enemyID = EnemyID.enumSize;
        
        if(gameObject.tag != "Player"){
            enemyID = GetComponent<EnemyStats>().enemyID;
            // If a boss enemy (update int if we add more bosses)
            if( enemyID == EnemyID.TimeLich || enemyID == EnemyID.BeetleBoss || enemyID == EnemyID.Harvester ){
                isBossEnemy = true;
            }
            // If normal enemy
            else{
                enemyHealthUI = gameObject.GetComponentInChildren<EnemyHealthBar>();
                if(enemyHealthUI == null){
                    Debug.LogError("No enemy health UI found for enemy unit: " + enemyID + "  Transform: " + gameObject.transform.position);
                    return;
                }
            }
            SetMaxHealthUI();
            SetCurrentHealthUI();
        }
    }

    public void SetStartingHealthUI()
    {
        // if(gameObject.tag == "Player"){
        //     Debug.Log("Max Health At Start: " + maxHitpoints + "\nCurrent Health At Start: " + currentHitpoints);
        // }

        SetMaxHealthUI();
        SetCurrentHealthUI();
    }

    public bool Damage(float damage)
    {
        if( gameObject.tag == "Player" && tempPlayerGodModeToggle ){
            return currentHitpoints <= 0;
        }

        currentHitpoints -= damage;
        OnHit.Invoke(this, damage);
        InGameUIManager.instance.ShowFloatingText(damage.ToString(), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1.5f, gameObject, "damage");
        // Debug.Log("Hitpoints");
        // Debug.Log(currentHitpoints);
        
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

    public void UpdateHealthOnUpgrade()
    {
        float oldMax = maxHitpoints;

        // Calculate new max from bonuses
        maxHitpoints = Player.instance.stats.getMaxHitPoints();

        // Calculate new current by adding the same amount you gained or lost
        currentHitpoints += maxHitpoints - oldMax;

        SetMaxHealthUI();
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
            // Tell the story manager that the player was killed by a creature
            // TODO: Get the enemyID of the creature who killed you!!!
            // StoryManager.instance.KilledEventOccurred(enemyID, StoryBeatType.KilledBy);            
            GameManager.instance.playerDeath = true;
        }
        else
        {
            if(isBossEnemy){
                InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(false);

                if(enemyID == EnemyID.TimeLich){
                    GameManager.instance.hasKilledTimeLich = true;
                }
            }

            if(enemyID == EnemyID.Slime || enemyID == EnemyID.TimeLich || enemyID == EnemyID.BeetleBoss){
                // Tell the story manager that this creature was killed
                StoryManager.instance.KilledEventOccurred(enemyID, StoryBeatType.EnemyKilled);
            }
            
            if(enemyDropGenerator){
                enemyDropGenerator.GetDrop(GameManager.instance.bossesKilled, transform);
            }
            else{
                Debug.LogWarning("No enemy drop generator found for enemy: " + enemyID);
            }
            
            for(int i = 0; i < TEMPCOINDROPAMOUNT; ++i)
            {
                Instantiate(coinPrefab, transform.position, Quaternion.identity);
            }

            if(Random.value <= TEMPSTARSHARDDROPCHANCE)
                Instantiate(starShardPrefab, transform.position, Quaternion.identity);

            // If you killed the mini boss, trigger Stellan's comm to tell you to go to the elevator
            if(enemyID == EnemyID.BeetleBoss){
                DialogueManager.instance.SetStellanCommTriggered(true);
                Player.instance.StartAutoDialogueFromPlayer();
            }

            Destroy(gameObject);
        }
        
    }

    private void onPlayerCrit(EntityHealth health, float critDamage)
    {
        InGameUIManager.instance.ShowFloatingText(critDamage.ToString(), 35, transform.position + (Vector3.up * 3), Vector3.up * 25, 1.5f, gameObject, "crit");
    }
}
