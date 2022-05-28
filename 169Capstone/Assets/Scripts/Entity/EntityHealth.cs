using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum DamageSourceType{
    Player,

    // Enemies
    Slime,
    Robert,

    // Bosses
    BeetleBoss,
    // Harvester,
    TimeLich,
    DefeatedTimeLichEndRunDeath,

    // Traps
    DeathPit,   // Intentionally does not contain "trap" because we check if the string contains "trap" to deal with trap damage
    TurretTrap,
    CoilTrap,
    FlameTrap,
    FloorSpikeTrap,

    DeathPitTimeLichArena,

    enumSize
}

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

    private DamageSourceType damageSourceCausedPlayerDeath;

    private const int TEMPCOINDROPAMOUNT = 5;
    private const float TEMPSTARSHARDDROPCHANCE = 0.25f;

    public bool tempPlayerGodModeToggle = false;    // FOR TESTING - REMOVE THIS FOR FINAL BUILD

    void Awake()
    {
        isBossEnemy = false;
        enemyID = EnemyID.enumSize;
        damageSourceCausedPlayerDeath = DamageSourceType.enumSize;
        
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

    void Start()
    {
        OnDeath.AddListener(onEntityDeath);
        OnCrit.AddListener(onPlayerCrit);        
    }

    public void SetStartingHealthUI()
    {
        SetMaxHealthUI();
        SetCurrentHealthUI();
    }

    // Need a bool to check for slime pit otherwise you could DODGE falling in a slime pit and fall for eternity
    public bool Damage(float damage, DamageSourceType damageSource)
    {
        if( gameObject.tag == "Player" && damageSource != DamageSourceType.DeathPit && damageSource != DamageSourceType.DefeatedTimeLichEndRunDeath ){
            // TEMP for dev
            if( tempPlayerGodModeToggle ){
                return currentHitpoints <= 0;
            }

            // Check for dodge
            float dodgeChance = Player.instance.stats.getDodgeChance();
            float rolledChance = Random.Range(0.0f, 1f);
            if( rolledChance <= dodgeChance ){
                // TODO: Trigger dodge floating text!
                return currentHitpoints <= 0;
            }

            // If we got to this point, lower damage according to defense before enacting the damage to the player
            float defense = Player.instance.stats.getDefense();
            damage *= 1 - defense;

            // IF A TRAP, account for trap damage resist
            if( damageSource.ToString().Contains("Trap") ){
                float trapDamageResist = Player.instance.stats.getTrapDamageResist();
                damage *= 1 - trapDamageResist;
            }
        }

        currentHitpoints -= damage;
        OnHit.Invoke(this, damage);
        if (gameObject.tag == "Player")
            InGameUIManager.instance.ShowFloatingText(damage.ToString(), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1.5f, gameObject, "damage-player");
        else
            InGameUIManager.instance.ShowFloatingText(damage.ToString(), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1.5f, gameObject, "damage-enemy");
        
        SetCurrentHealthUI();

        if(currentHitpoints <= 0)
        {
            currentHitpoints = 0;

            // If the player died in the lich arena but NOT to the lich himself OR after killing him OR to a death pit, set damageSource to lich instead
            if( GameManager.instance.currentSceneName == GameManager.LICH_ARENA_STRING_NAME && damageSource != DamageSourceType.DeathPitTimeLichArena && damageSource != DamageSourceType.DefeatedTimeLichEndRunDeath ){
                damageSourceCausedPlayerDeath = DamageSourceType.TimeLich;
            }
            else{   // Otherwise, set it to whatever it registered as
                damageSourceCausedPlayerDeath = damageSource;
            }
            
            OnDeath.Invoke(this);
        }

        return currentHitpoints <= 0;
    }

    public void Heal(float health)
    {
        currentHitpoints += health;
        InGameUIManager.instance.ShowFloatingText(health.ToString(), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1.5f, gameObject, "health");

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

        InGameUIManager.instance.ShowFloatingText(maxHitpoints.ToString(), 30, transform.position + (Vector3.up * 3), Vector3.up * 100, 1.5f, gameObject, "health");

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

        // Debug.Log("Death");
        if(gameObject.tag == "Player")
        {
            // Tell the story manager what the player was killed by
            StoryManager.instance.KilledEventOccurred(damageSourceCausedPlayerDeath, StoryBeatType.KilledBy);            
            GameManager.instance.playerDeath = true;
        }
        else
        {
            // Tell the story manager that this creature was killed
            StoryManager.instance.KilledEventOccurred(enemyID, StoryBeatType.EnemyKilled);

            if(isBossEnemy){
                InGameUIManager.instance.bossHealthBar.SetBossHealthBarActive(false);

                if(enemyID == EnemyID.TimeLich){
                    // Story state things
                    GameManager.instance.hasKilledTimeLich = true;
                    DialogueManager.instance.SetTimeLichDeathDialogueTriggered(true);
                    if( PermanentUpgradeManager.instance.GetSkillLevel(PermanentUpgradeType.TimeLichKillerThing) > 0 ){
                        GameManager.instance.epilogueTriggered = true;
                    }

                    // Trigger auto dialogue
                    Player.instance.StartAutoDialogueFromPlayer();
                }
            }
            
            // If we're NOT in the lich fight, maybe drop something
            if(GameManager.instance.currentSceneName != GameManager.LICH_ARENA_STRING_NAME){
                if(enemyDropGenerator)
                    enemyDropGenerator.GetDrop(GameManager.instance.bossesKilled, transform);
                else
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
